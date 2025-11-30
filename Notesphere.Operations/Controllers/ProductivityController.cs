using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notesphere.Entities.ProductivityModels;
using Notesphere.Operations.Models.Productivity;
using Notesphere.Services.ProductivityRepository;

namespace Notesphere.Operations.Controllers
{
    public class ProductivityController : Controller
    {
        private readonly IProductivityServices _productivityServices;

        public ProductivityController(IProductivityServices productivityServices)
        {
            _productivityServices = productivityServices;
        }

        // Temporary helper - replaced with actual user log-in once Identity is wired
        private int GetCurrentStudentUserId()
        {
            return 1; // demo user 
        }

        // Shows all tasks for current user
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentStudentUserId();
            var tasks = await _productivityServices.GetTasksForUserAsync(userId);

            var vm = tasks.Select(t => new ProductivityTaskListItemVM
            {
                // Map Entity -> List View Model

                Id = t.Id,
                Title = t.Title,
                Priority = t.Priority,
                Status = t.Status,
                DueDate = t.DueDate,
                ProgressPercent = t.ProgressPercent
            }).ToList();

            return View(vm);
        }

        // Show empty Create form

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new ProductivityTaskFormVM();
            return View(vm);
        }

        // Handles submission of new task

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductivityTaskFormVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = GetCurrentStudentUserId();

            var entity = new ProductivityTask
            {
                // Map form -> Entity

                Title = vm.Title,
                Description = vm.Description,
                DueDate = vm.DueDate,
                Priority = vm.Priority,
                Status = vm.Status,
                StudentUserId = userId
            };

            await _productivityServices.AddTaskAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        // Loads task data into edit form

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _productivityServices.GetTaskByIdAsync(id);
            if (entity == null) return NotFound();

            var vm = new ProductivityTaskFormVM
            {
                // Map entity -> edit form VM

                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                Priority = entity.Priority,
                Status = entity.Status,
                ChecklistItems = entity.ChecklistItems
                    .OrderBy(c => c.Order)
                    .Select(c => new TaskChecklistItemVM
                    {
                        Id = c.Id,
                        Label = c.Label,
                        IsDone = c.IsDone
                    }).ToList()
            };

            return View(vm);
        }

        // Saves updated task details

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductivityTaskFormVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = await _productivityServices.GetTaskByIdAsync(vm.Id);
            if (entity == null) return NotFound();

            // Update entity fields

            entity.Title = vm.Title;
            entity.Description = vm.Description;
            entity.DueDate = vm.DueDate;
            entity.Priority = vm.Priority;
            entity.Status = vm.Status;

            await _productivityServices.UpdateTaskAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        // Removes a task

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productivityServices.DeleteTaskAsync(id);
            return RedirectToAction(nameof(Index));
        }

       

        // Adds a new checklist step to a task

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChecklistItem(int taskId, string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                return RedirectToAction(nameof(Edit), new { id = taskId });

            // set order as last + 1
            var existing = await _productivityServices.GetChecklistForTaskAsync(taskId);
            var item = new TaskChecklistItem
            {
                ProductivityTaskId = taskId,
                Label = label,
                IsDone = false,
                Order = existing.Any() ? existing.Max(c => c.Order) + 1 : 1
            };

            await _productivityServices.AddChecklistItemAsync(item);
            return RedirectToAction(nameof(Edit), new { id = taskId });
        }

        // Marks a checklist item as done/undone and updates progress

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleChecklistItem(int taskId, int itemId, bool isDone)
        {
            await _productivityServices.ToggleChecklistItemAsync(itemId, isDone);
            return RedirectToAction(nameof(Edit), new { id = taskId });
        }

        // Deletes a checklist item and recalculates task progress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChecklistItem(int taskId, int itemId)
        {
            await _productivityServices.DeleteChecklistItemAsync(itemId);
            return RedirectToAction(nameof(Edit), new { id = taskId });
        }
    }
}

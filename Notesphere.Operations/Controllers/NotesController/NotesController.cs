using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Notesphere.Entities.NotesModels;
using Notesphere.Services.NotesRepository;

namespace Notesphere.Operations.Controllers
{
    public class NotesController : Controller
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }

        // Helper: populate StudentUser dropdown
        private async Task PopulateStudentUserDropDown(object? selectedId = null)
        {
            var users = await _notesService.GetStudentUsers();
            ViewData["StudentUserId"] = new SelectList(users, "Id", "Email", selectedId);
        }

        // Helper: populate templates list
        private async Task PopulateTemplates(object? selectedId = null)
        {
            var templates = await _notesService.GetTemplatesAsync();
            ViewBag.Templates = templates;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            var notes = await _notesService.GetAllNotes();
            return View(notes);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            return View(note);
        }

        // GET: Notes/Create
        public async Task<IActionResult> Create()
        {
            await PopulateStudentUserDropDown();
            await PopulateTemplates();
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentUserId,Title,Content,IsFavorite,TemplateId,CreatedAt,UpdatedAt")] Note note)
        {
            if (ModelState.IsValid)
            {
                await _notesService.AddNote(note);
                return RedirectToAction(nameof(Index));
            }

            await PopulateStudentUserDropDown(note.StudentUserId);
            await PopulateTemplates(note.TemplateId);
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            await PopulateStudentUserDropDown(note.StudentUserId);
            await PopulateTemplates(note.TemplateId);

            return View(note);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentUserId,Title,Content,IsFavorite,TemplateId,CreatedAt,UpdatedAt")] Note note)
        {
            if (id != note.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _notesService.UpdateNote(note);
                return RedirectToAction(nameof(Index));
            }

            await PopulateStudentUserDropDown(note.StudentUserId);
            await PopulateTemplates(note.TemplateId);

            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _notesService.DeleteNote(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

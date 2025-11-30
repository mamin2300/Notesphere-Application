using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Notesphere.Entities.NotesModels;
using Notesphere.Services.NotesRepository;
using System.Security.Claims;

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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var note = new Note();
            await PopulateTemplates(null);
            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
            {
                // need templates again if the form re-renders
                await PopulateTemplates(note.TemplateId);
                return View(note);
            }

            // Get logged-in user
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            note.StudentUserId = int.Parse(userIdString);

            await _notesService.AddNote(note);  // your existing repo method

            // go straight to the editor for this note
            return RedirectToAction("Editor", new { id = note.Id });
        }


        // GET: Notes/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            await PopulateStudentUserDropDown(note.StudentUserId);
            await PopulateTemplates(note.TemplateId);

            return View(note);
        }

        // POST: Notes/Edit
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

        // GET: Notes/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            return View(note);
        }

        // POST: Notes/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _notesService.DeleteNote(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editor(int id)
        {
            var note = await _notesService.GetNoteById(id);
            if (note == null) return NotFound();

            var pages = await _notesService.GetPagesByNoteId(id);
            ViewBag.Pages = pages;

            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> SavePage(int noteId, int pageNumber, string imageData)
        {
            await _notesService.SavePageImage(noteId, pageNumber, imageData);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SaveText(int noteId, string content)
        {
            var note = await _notesService.GetNoteById(noteId);
            if (note == null) return NotFound();

            note.Content = content;
            await _notesService.UpdateNote(note);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddPage(int noteId)
        {
            int newPage = await _notesService.AddNewPage(noteId);
            return Json(new { pageNumber = newPage });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(int noteId, int pageNumber)
        {
            await _notesService.DeletePage(noteId, pageNumber);
            return Ok();
        }

    }
}

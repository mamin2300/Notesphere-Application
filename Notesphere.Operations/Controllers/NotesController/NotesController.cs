using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Notesphere.Entities.NotesModels;
using Notesphere.Services.NotesRepository;


namespace Notesphere.Operations.Controllers
{
    /// Handles CRUD operations for notes and integrates with the
    /// drawing notebook editor (multi-page canvas + text).
    /// Author: Mamin Khan
    [Authorize]
    public class NotesController : Controller
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }

        // ----------------- Helpers -----------------

        private int? GetCurrentStudentUserId()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idString, out var id))
                return id;
            return null;
        }

        // Dropdown of all student users (mainly for admin/edit view)
        private async Task PopulateStudentUserDropDown(object? selectedId = null)
        {
            var users = await _notesService.GetStudentUsers();
            ViewData["StudentUserId"] = new SelectList(users, "Id", "Email", selectedId);
        }

        // Template list for select / radio buttons
        private async Task PopulateTemplatesAsync(object? selectedId = null)
        {
            var templates = await _notesService.GetTemplatesAsync();
            ViewBag.Templates = templates;
            ViewBag.SelectedTemplateId = selectedId;
        }

        // ----------------- Views -----------------

        // GET: /Notes
        public async Task<IActionResult> Index()
        {
            var studentId = GetCurrentStudentUserId();
            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var notes = await _notesService.GetNotesForStudentAsync(studentId.Value);
            return View(notes);
        }

        // GET: /Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            return View(note);
        }

        // GET: /Notes/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var note = new Note();
            await PopulateTemplatesAsync(null);
            return View(note);
        }

        // POST: /Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTemplatesAsync(note.TemplateId);
                return View(note);
            }

            var studentId = GetCurrentStudentUserId();
            if (studentId == null)
                return RedirectToAction("Login", "Account");

            note.StudentUserId = studentId.Value;

            await _notesService.AddNote(note);   // creates initial version inside service
            return RedirectToAction("Editor", new { id = note.Id });
        }

        // GET: /Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            await PopulateStudentUserDropDown(note.StudentUserId);
            await PopulateTemplatesAsync(note.TemplateId);

            return View(note);
        }

        // POST: /Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StudentUserId,Title,Content,IsFavorite,TemplateId,CreatedAt,UpdatedAt")]
            Note note)
        {
            if (id != note.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateStudentUserDropDown(note.StudentUserId);
                await PopulateTemplatesAsync(note.TemplateId);
                return View(note);
            }

            await _notesService.UpdateNote(note);  // service will create NoteVersion snapshot
            return RedirectToAction(nameof(Index));
        }

        // GET: /Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var note = await _notesService.GetNoteById(id.Value);
            if (note == null) return NotFound();

            return View(note);
        }

        // POST: /Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _notesService.DeleteNote(id);
            return RedirectToAction(nameof(Index));
        }

        // ----------------- Notebook Editor -----------------

        // GET: /Notes/Editor/5
        public async Task<IActionResult> Editor(int id)
        {
            var note = await _notesService.GetNoteById(id);
            if (note == null) return NotFound();

            var pages = await _notesService.GetPagesByNoteId(id);
            ViewBag.Pages = pages;

            return View(note);
        }

        // POST: /Notes/SavePage   (called via AJAX from canvas)
        [HttpPost]
        public async Task<IActionResult> SavePage(int noteId, int pageNumber, string imageData)
        {
            await _notesService.SavePageImage(noteId, pageNumber, imageData);
            return Ok();
        }

        // POST: /Notes/SaveText   (called via AJAX from text panel)
        [HttpPost]
        public async Task<IActionResult> SaveText(int noteId, string content)
        {
            var note = await _notesService.GetNoteById(noteId);
            if (note == null) return NotFound();

            note.Content = content;
            await _notesService.UpdateNote(note); // creates NoteVersion

            return Ok();
        }

        // POST: /Notes/AddPage
        [HttpPost]
        public async Task<IActionResult> AddPage(int noteId)
        {
            int newPage = await _notesService.AddNewPage(noteId);
            return Json(new { pageNumber = newPage });
        }

        // POST: /Notes/DeletePage
        [HttpPost]
        public async Task<IActionResult> DeletePage(int noteId, int pageNumber)
        {
            await _notesService.DeletePage(noteId, pageNumber);
            return Ok();
        }
    }
}

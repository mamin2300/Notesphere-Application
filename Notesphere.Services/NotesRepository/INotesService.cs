using Notesphere.Entities.NotesModels;

namespace Notesphere.Services.NotesRepository
{
    // Business/service layer contract for all note-related use cases.
    public interface INotesService
    {
        // ---------------- BASIC NOTE CRUD ----------------

        Task<List<Note>> GetAllNotes();  // Return all notes (filter by user in controller).
        Task<Note?> GetNoteById(int id);  // Get a single note by primary key.
        Task AddNote(Note note);  // Create a new note (CreatedAt/UpdatedAt are set in the repository).
        Task UpdateNote(Note note);  //Update an existing note + automatically create a NoteVersion snapshot.
        Task DeleteNote(int id);  //Delete a note by Id (will cascade to related rows if configured).
        Task<List<Note>> GetNotesForStudentAsync(int studentUserId);  // Convenience: get notes for a student (used in dashboard/list pages).

        // ---------------- TEMPLATES ----------------

        Task<List<NoteTemplate>> GetTemplatesAsync();
        Task<NoteTemplate?> GetTemplateByIdAsync(int id);

        // ---------------- TAGS & NOTE TAGS ----------------

        Task<List<Tag>> GetAllTags();  //Return all available tags.
        Task AddTagToNote(NoteTag join);  //Add a tag to a note (avoids duplicates)
        Task<List<Tag>> GetTagsForNoteAsync(int noteId);  //Get tags for a specific note.

        // ---------------- VERSION HISTORY ----------------

        Task SaveNoteVersion(NoteVersion version);  // Save a new version snapshot. If VersionNumber or SavedAt are not set, the repository will fill them in.
        Task<List<NoteVersion>> GetVersionsForNoteAsync(int noteId);  //Get all versions for a note, newest first.

        // ---------------- EXPORT LOG ----------------
        Task SavePage(int noteId, int pageNumber, string imageData, string textBoxes);

        Task LogNoteExportAsync(NoteExport export);  // Log that a note was exported in a certain format/destination.
        Task<List<NoteExport>> GetExportsForNoteAsync(int noteId);  // Get all exports for a note.

        // ---------------- STUDENT USERS ----------------

        Task<List<StudentUser>> GetStudentUsers();  // Used mainly for dropdowns / admin.

        // ---------------- PAGES (DRAWING) ----------------

        Task<List<NotePage>> GetPagesByNoteId(int noteId);
        Task SavePageImage(int noteId, int pageNumber, string imageData);  // Insert or update a page image for a given note + page number.
        Task<int> AddNewPage(int noteId);  // Add a new page at the end and return the new page number.
        Task DeletePage(int noteId, int pageNumber);  // Delete a specific page for a note.
    }
}

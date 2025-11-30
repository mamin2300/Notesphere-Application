using Notesphere.Entities.NotesModels;

namespace Notesphere.Services.NotesRepository
{
    /// <summary>
    /// Business/service layer contract for all note-related use cases.
    /// </summary>
    public interface INotesService
    {
        // ---------------- BASIC NOTE CRUD ----------------

        /// <summary>Return all notes (you can later filter by user in controller).</summary>
        Task<List<Note>> GetAllNotes();

        /// <summary>Get a single note by primary key.</summary>
        Task<Note?> GetNoteById(int id);

        /// <summary>Create a new note (CreatedAt/UpdatedAt are set in the repository).</summary>
        Task AddNote(Note note);

        /// <summary>Update an existing note + automatically create a NoteVersion snapshot.</summary>
        Task UpdateNote(Note note);

        /// <summary>Delete a note by Id (will cascade to related rows if configured).</summary>
        Task DeleteNote(int id);

        // Convenience: get notes for a student (used in dashboard/list pages).
        Task<List<Note>> GetNotesForStudentAsync(int studentUserId);


        // ---------------- TEMPLATES ----------------

        Task<List<NoteTemplate>> GetTemplatesAsync();
        Task<NoteTemplate?> GetTemplateByIdAsync(int id);


        // ---------------- TAGS & NOTE TAGS ----------------

        /// <summary>Return all available tags.</summary>
        Task<List<Tag>> GetAllTags();

        /// <summary>Add a tag to a note (avoids duplicates).</summary>
        Task AddTagToNote(NoteTag join);

        /// <summary>Get tags for a specific note.</summary>
        Task<List<Tag>> GetTagsForNoteAsync(int noteId);


        // ---------------- VERSION HISTORY ----------------

        /// <summary>
        /// Save a new version snapshot. If VersionNumber or SavedAt are not set,
        /// the repository will fill them in.
        /// </summary>
        Task SaveNoteVersion(NoteVersion version);

        /// <summary>Get all versions for a note, newest first.</summary>
        Task<List<NoteVersion>> GetVersionsForNoteAsync(int noteId);


        // ---------------- EXPORT LOG ----------------

        /// <summary>Log that a note was exported in a certain format/destination.</summary>
        Task LogNoteExportAsync(NoteExport export);

        /// <summary>Get all exports for a note.</summary>
        Task<List<NoteExport>> GetExportsForNoteAsync(int noteId);


        // ---------------- STUDENT USERS ----------------

        /// <summary>Used mainly for dropdowns / admin.</summary>
        Task<List<StudentUser>> GetStudentUsers();


        // ---------------- PAGES (DRAWING) ----------------

        Task<List<NotePage>> GetPagesByNoteId(int noteId);

        /// <summary>Insert or update a page image for a given note + page number.</summary>
        Task SavePageImage(int noteId, int pageNumber, string imageData);

        /// <summary>Add a new page at the end and return the new page number.</summary>
        Task<int> AddNewPage(int noteId);

        /// <summary>Delete a specific page for a note.</summary>
        Task DeletePage(int noteId, int pageNumber);
    }
}

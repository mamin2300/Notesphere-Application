using Notesphere.Entities.NotesModels;  

namespace Notesphere.Services.NotesRepository
{
    public interface INotesphereService
    {
        // Notes CRUD operations
        Task<List<Note>> GetAllNotes();
        Task<Note?> GetNoteById(int id);
        Task AddNote(Note note);
        Task UpdateNote(Note note);
        Task DeleteNote(int id);

        Task<List<NoteTemplate>> GetTemplates();
        Task<List<Tag>> GetAllTags();
        Task SaveNoteVersion(NoteVersion Version);
        Task AddTagToNote(NoteTag join);

        //Planner CRUD operations
        //Sharing CRUD operations
        //Productivity CRUD operations
    }
}

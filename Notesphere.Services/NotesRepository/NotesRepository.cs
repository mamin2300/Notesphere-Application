using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.NotesModels;
using Notesphere.Services.NotesphereDataAccessLayer;

namespace Notesphere.Services.NotesRepository
{
    public class NotesRepository : INotesService
    {
        private readonly NotesphereDbContext _db;

        public NotesRepository(NotesphereDbContext db)
        {
            _db = db;
        }
        // Notes CRUD operations

        public async Task<List<Note>> GetAllNotes() =>
            await _db.Notes.ToListAsync();

        public async Task<Note?> GetNoteById(int id) =>
            await _db.Notes.FirstOrDefaultAsync(n => n.Id == id);

        public async Task AddNote(Note note)
        {
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateNote(Note note)
        {
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteNote(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note != null)
            {
                _db.Notes.Remove(note);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<NoteTemplate>> GetTemplatesAsync()
        {
            return await _db.NoteTemplates
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<NoteTemplate?> GetTemplateByIdAsync(int id)
        {
            return await _db.NoteTemplates.FirstOrDefaultAsync(t => t.Id == id);
        }


        public async Task<List<Tag>> GetAllTags() =>
            await _db.Tags.ToListAsync();

        public async Task SaveNoteVersion(NoteVersion version)
        {
            _db.NoteVersions.Add(version);
            await _db.SaveChangesAsync();
        }

        public async Task AddTagToNote(NoteTag join)
        {
            _db.NoteTags.Add(join);
            await _db.SaveChangesAsync();
        }

        public async Task<List<StudentUser>> GetStudentUsers() =>
            await _db.Set<StudentUser>().ToListAsync();
    }
}

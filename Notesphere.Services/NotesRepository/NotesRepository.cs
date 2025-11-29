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


        public async Task<List<NotePage>> GetPagesByNoteId(int noteId)
        {
            return await _db.NotePages
                .Where(p => p.NoteId == noteId)
                .OrderBy(p => p.PageNumber)
                .ToListAsync();
        }

        public async Task SavePageImage(int noteId, int pageNumber, string imageData)
        {
            var page = await _db.NotePages
                .FirstOrDefaultAsync(p => p.NoteId == noteId && p.PageNumber == pageNumber);

            if (page == null)
            {
                page = new NotePage
                {
                    NoteId = noteId,
                    PageNumber = pageNumber,
                    ImageData = imageData,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.NotePages.Add(page);
            }
            else
            {
                page.ImageData = imageData;
                page.UpdatedAt = DateTime.UtcNow;
                _db.NotePages.Update(page);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<int> AddNewPage(int noteId)
        {
            int nextPage = await _db.NotePages
                .Where(p => p.NoteId == noteId)
                .CountAsync() + 1;

            _db.NotePages.Add(new NotePage
            {
                NoteId = noteId,
                PageNumber = nextPage,
                ImageData = "",
                UpdatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return nextPage;
        }

        public async Task DeletePage(int noteId, int pageNumber)
        {
            var page = await _db.NotePages
                .FirstOrDefaultAsync(p => p.NoteId == noteId && p.PageNumber == pageNumber);

            if (page != null)
            {
                _db.NotePages.Remove(page);
                await _db.SaveChangesAsync();
            }
        }


    }
}

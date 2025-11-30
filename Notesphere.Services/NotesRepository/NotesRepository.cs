using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.NotesModels;
using Notesphere.Services.NotesphereDataAccessLayer;

namespace Notesphere.Services.NotesRepository
{
    /// <summary>
    /// Concrete implementation of INotesService.
    /// Encapsulates all data access + business rules for Notes.
    /// </summary>
    public class NotesRepository : INotesService
    {
        private readonly NotesphereDbContext _db;

        public NotesRepository(NotesphereDbContext db)
        {
            _db = db;
        }

        // -------------------------------------------------
        // BASIC NOTE CRUD
        // -------------------------------------------------

        public async Task<List<Note>> GetAllNotes()
        {
            // include StudentUser + Template to be more useful in views
            return await _db.Notes
                .Include(n => n.StudentUser)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<List<Note>> GetNotesForStudentAsync(int studentUserId)
        {
            return await _db.Notes
                .Where(n => n.StudentUserId == studentUserId)
                .Include(n => n.StudentUser)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Note?> GetNoteById(int id)
        {
            return await _db.Notes
                .Include(n => n.StudentUser)
                .Include(n => n.Pages)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddNote(Note note)
        {
            // set timestamps here so controller stays clean
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = DateTime.UtcNow;

            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            // create first version snapshot
            var version = new NoteVersion
            {
                NoteId = note.Id,
                Title = note.Title,
                Content = note.Content,
                VersionNumber = 1,
                SavedAt = DateTime.UtcNow
            };

            _db.NoteVersions.Add(version);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateNote(Note note)
        {
            // refresh UpdatedAt
            note.UpdatedAt = DateTime.UtcNow;
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();

            // automatically create a new NoteVersion
            await SaveNoteVersion(new NoteVersion
            {
                NoteId = note.Id,
                Title = note.Title,
                Content = note.Content
                // VersionNumber + SavedAt will be filled inside SaveNoteVersion
            });
        }

        public async Task DeleteNote(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return;

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
        }

        // -------------------------------------------------
        // TEMPLATES
        // -------------------------------------------------

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

        // -------------------------------------------------
        // TAGS + NOTE TAGS
        // -------------------------------------------------

        public async Task<List<Tag>> GetAllTags()
        {
            return await _db.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task AddTagToNote(NoteTag join)
        {
            // avoid duplicate (NoteId, TagId) rows
            bool exists = await _db.NoteTags
                .AnyAsync(nt => nt.NoteId == join.NoteId && nt.TagId == join.TagId);

            if (!exists)
            {
                _db.NoteTags.Add(join);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Tag>> GetTagsForNoteAsync(int noteId)
        {
            return await _db.NoteTags
                .Where(nt => nt.NoteId == noteId)
                .Join(
                    _db.Tags,
                    nt => nt.TagId,
                    t => t.Id,
                    (nt, t) => t
                )
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        // -------------------------------------------------
        // VERSION HISTORY
        // -------------------------------------------------

        public async Task SaveNoteVersion(NoteVersion version)
        {
            // if VersionNumber is 0, compute the next version
            if (version.VersionNumber <= 0)
            {
                var last = await _db.NoteVersions
                    .Where(v => v.NoteId == version.NoteId)
                    .OrderByDescending(v => v.VersionNumber)
                    .FirstOrDefaultAsync();

                version.VersionNumber = (last?.VersionNumber ?? 0) + 1;
            }

            if (version.SavedAt == default)
            {
                version.SavedAt = DateTime.UtcNow;
            }

            // make sure Title / Content are not null
            version.Title ??= string.Empty;
            version.Content ??= string.Empty;

            _db.NoteVersions.Add(version);
            await _db.SaveChangesAsync();
        }

        public async Task<List<NoteVersion>> GetVersionsForNoteAsync(int noteId)
        {
            return await _db.NoteVersions
                .Where(v => v.NoteId == noteId)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();
        }

        // -------------------------------------------------
        // EXPORT LOG
        // -------------------------------------------------

        public async Task LogNoteExportAsync(NoteExport export)
        {
            if (export.ExportedAt == default)
            {
                export.ExportedAt = DateTime.UtcNow;
            }

            export.Format ??= string.Empty;
            export.Destination = string.IsNullOrWhiteSpace(export.Destination)
                ? null
                : export.Destination.Trim();

            _db.NoteExports.Add(export);
            await _db.SaveChangesAsync();
        }

        public async Task<List<NoteExport>> GetExportsForNoteAsync(int noteId)
        {
            return await _db.NoteExports
                .Where(e => e.NoteId == noteId)
                .OrderByDescending(e => e.ExportedAt)
                .ToListAsync();
        }

        // -------------------------------------------------
        // STUDENT USERS
        // -------------------------------------------------

        public async Task<List<StudentUser>> GetStudentUsers()
        {
            return await _db.Set<StudentUser>()
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        // -------------------------------------------------
        // PAGES (DRAWING)
        // -------------------------------------------------

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
            // next page number = current count + 1
            int nextPage = await _db.NotePages
                .Where(p => p.NoteId == noteId)
                .CountAsync() + 1;

            var page = new NotePage
            {
                NoteId = noteId,
                PageNumber = nextPage,
                ImageData = string.Empty,
                UpdatedAt = DateTime.UtcNow
            };

            _db.NotePages.Add(page);
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

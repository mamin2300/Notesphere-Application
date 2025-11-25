using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.NotesModels;
namespace Notesphere.Services.NotesDataAccessLayer
{
    public class NotesDbContext:DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }
        public DbSet<NoteTemplate> NoteTemplates { get; set; }
        public DbSet<NoteVersion> NoteVersions { get; set; }
        public DbSet<NoteExport> NoteExports { get; set; }
    }
}

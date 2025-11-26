using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.NotesModels;

namespace Notesphere.Services.NotesphereDataAccessLayer
{
    public class NotesphereDbContext : DbContext
    {
        public NotesphereDbContext(DbContextOptions<NotesphereDbContext> options) : base(options)
        {
        }

        //notes.db sets 
        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }
        public DbSet<NoteTemplate> NoteTemplates { get; set; }
        public DbSet<NoteVersion> NoteVersions { get; set; }
        public DbSet<NoteExport> NoteExports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // composite key for join table
            modelBuilder.Entity<NoteTag>()
                .HasKey(nt => new { nt.NoteId, nt.TagId });
        }

        //planner.db sets
        //sharing.db sets
        //productivity.db sets
    }
}


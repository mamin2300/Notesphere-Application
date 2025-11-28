using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.DashboardModels;
using Notesphere.Entities.NotesModels;
using Notesphere.Entities.PlannerModels;

namespace Notesphere.Services.NotesphereDataAccessLayer
{
    public class NotesphereDbContext : DbContext
    {
        public NotesphereDbContext(DbContextOptions<NotesphereDbContext> options) : base(options)
        {
        }

        //Dashboard.db sets
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<QuickActions> QuickActions { get; set; }
        public DbSet<Workspace>Workspaces { get; set; }

        //Notes.db sets 
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
        
        //Planner.db sets
        public DbSet<Event> Events { get; set; }
        public DbSet<RecurringEvent> RecurringEvents { get; set; }
        public DbSet<Conflict> Conflicts { get; set; }

        //Sharing.db sets

        //Productivity.db sets

    }
}


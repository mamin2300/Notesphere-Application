using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.DashboardModels;
using Notesphere.Entities.NotesModels;
using Notesphere.Entities.PlannerModels;
using Notesphere.Entities.SharingModels;
using Notesphere.Entities.ProductivityModels;

namespace Notesphere.Services.NotesphereDataAccessLayer
{
    public class NotesphereDbContext : DbContext
    {
        public NotesphereDbContext(DbContextOptions<NotesphereDbContext> options) : base(options)
        {
        }

        // ---------------- DASHBOARD ----------------
        public DbSet<Reminder> Reminders { get; set; } 
        public DbSet<QuickActions> QuickActions { get; set; } 
        public DbSet<Workspace> Workspaces { get; set; } 

        // ---------------- NOTES ----------------
        public DbSet<StudentUser> StudentUser { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<NotePage> NotePages { get; set; } 
        public DbSet<NoteTemplate> NoteTemplates { get; set; } 
        public DbSet<NoteVersion> NoteVersions { get; set; } 
        public DbSet<NoteExport> NoteExports { get; set; } 
        public DbSet<Tag> Tags { get; set; } 
        public DbSet<NoteTag> NoteTags { get; set; } 

        //Planner.db sets
        public DbSet<Event> Events { get; set; }
        public DbSet<RecurringEvent> RecurringEvents { get; set; }
        public DbSet<Conflict> Conflicts { get; set; }

        //Sharing.db sets
        public DbSet<GroupSpace> GroupSpaces { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<SharedNote> SharedNotes { get; set; }
        public DbSet<NoteComment> NoteComments { get; set; }

        //Productivity.db sets
        public DbSet<ProductivityTask> ProductivityTasks { get; set; } = null!;
        public DbSet<TaskChecklistItem> TaskChecklistItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========= NOTES relationships (your part) =========

            // StudentUser (1) -> (many) Notes
            modelBuilder.Entity<Note>()
                .HasOne(n => n.StudentUser)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // NoteTemplate (1) -> (many) Notes  (optional TemplateId)
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Template)
                .WithMany(t => t.Notes)
                .HasForeignKey(n => n.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Note (1) -> (many) Pages
            modelBuilder.Entity<NotePage>()
                .HasOne(p => p.Note)
                .WithMany(n => n.Pages)
                .HasForeignKey(p => p.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note (1) -> (many) Versions
            modelBuilder.Entity<NoteVersion>()
                .HasOne(v => v.Note)
                .WithMany(n => n.Versions)
                .HasForeignKey(v => v.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note (1) -> (many) Exports
            modelBuilder.Entity<NoteExport>()
                .HasOne(e => e.Note)
                .WithMany(n => n.Exports)
                .HasForeignKey(e => e.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            // many-to-many: Note <-> Tag via NoteTag
            modelBuilder.Entity<NoteTag>()
                .HasKey(nt => new { nt.NoteId, nt.TagId });

            modelBuilder.Entity<NoteTag>()
                .HasOne(nt => nt.Note)
                .WithMany(n => n.NoteTags)
                .HasForeignKey(nt => nt.NoteId);

            modelBuilder.Entity<NoteTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NoteTags)
                .HasForeignKey(nt => nt.TagId);

            // ========= TEMPLATE seeding (your page styles) =====

            modelBuilder.Entity<NoteTemplate>().HasData(
                new NoteTemplate
                {
                    Id = 1,
                    Name = "Lined",
                    Description = "Simple ruled notebook page",
                    CssKey = "lined",
                    DefaultContent = "",
                    IsSystemTemplate = true
                },
                new NoteTemplate
                {
                    Id = 2,
                    Name = "Dotted",
                    Description = "Dot grid for bullet journaling",
                    CssKey = "dotted",
                    DefaultContent = "",
                    IsSystemTemplate = true
                },
                new NoteTemplate
                {
                    Id = 3,
                    Name = "Grid",
                    Description = "Box/grid style math page",
                    CssKey = "boxed",
                    DefaultContent = "",
                    IsSystemTemplate = true
                },
                new NoteTemplate
                {
                    Id = 4,
                    Name = "Blank",
                    Description = "Plain page without guides",
                    CssKey = "blank",
                    DefaultContent = "",
                    IsSystemTemplate = true
                }
            );
        }
    }
}

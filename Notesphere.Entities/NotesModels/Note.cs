using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    //Class Summary:
    // Represents a single note in the system.
    /// A note belongs to one student user, can have multiple pages,
    /// tags, versions (history) and exports (export log).

    public class Note
    {

        [Key]
        public int Id { get; set; } // Primary key for the Note entity.
        [Required]
        public int StudentUserId { get; set; } // Foreign key to the student user that owns this note.
        public StudentUser StudentUser { get; set; } = null!; // Navigation property to the owning student user.


        [Required]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty;  // Short title 
        public string Content { get; set; } = string.Empty;  // Plain-text content of the note (used by the text tool).
        public bool IsFavorite { get; set; }  //Marks the note as favorite so it appears in the favorites section.

        public int? TemplateId { get; set; }  //reference to a template (lined / dotted / grid).
        public NoteTemplate? Template { get; set; }  // Navigation property to the template being used.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date and time when the note was created (UTC).
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Date and time when the note was last updated (UTC).

        public ICollection<NotePage> Pages { get; set; } = new List<NotePage>(); // Collection of pages for this note (drawing pages).
        public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();  // Many-to-many join to tags. Each note can have many tags.
        public ICollection<NoteVersion> Versions { get; set; } = new List<NoteVersion>(); // Version history records for this note.
        public ICollection<NoteExport> Exports { get; set; } = new List<NoteExport>();  // Export log entries for this note (PDF export, etc.).

    }
}

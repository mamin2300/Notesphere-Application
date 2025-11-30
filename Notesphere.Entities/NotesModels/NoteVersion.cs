using System;
using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    // Represents a historical snapshot of a note.
    // Each time the note is updated, a new version can be stored.
    public class NoteVersion
    {
        [Key]
        public int Id { get; set; }  // Primary key for the version record.

        public int NoteId { get; set; }  // Foreign key to the original Note.
        public Note Note { get; set; } = null!;  // Navigation back to the Note.

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty;  // Title of the note at the time this version was saved.

        [Required]
        [StringLength(5000, ErrorMessage = "Content is too long.")]
        public string Content { get; set; } = string.Empty;  // Content of the note at the time this version was saved.


        [Range(1, int.MaxValue, ErrorMessage = "Version number must be at least 1.")]
        public int VersionNumber { get; set; }  // Incremental version number (1, 2, 3...) for this note.

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;  // Timestamp when this version was created.
    }
}

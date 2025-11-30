using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    // Class Summary:
    // Represents a single page within a multi-page note.
    // Stores both the page number and the drawing/image data.
    public class NotePage
    {
        [Key]
        public int Id { get; set; }  // Primary key of the NotePage entity.

        [Required]
        public int NoteId { get; set; }  // Foreign key linking this page to its parent note.

        [Required]
        public Note? Note { get; set; }  // Navigation property to the parent Note.

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
        public int PageNumber { get; set; } //Page number inside the note (1, 2, 3...).

        [Required(ErrorMessage = "Page image data cannot be empty.")]
        public string ImageData { get; set; } = string.Empty;  // Base64 encoded PNG/JPEG data of the drawn page.

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Timestamp for the most recent modification.
    }
}

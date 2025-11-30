using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    //class summary: 
    // Represents a single export action for a note.
    // Used to track when a note was exported (PDF, image, etc.)
    // for history and auditing purposes.
    public class NoteExport
    {

        [Key]
        public int Id { get; set; } // Primary key of the NoteExport entity.

        [Required]
        public int NoteId { get; set; }  // Foreign key to the note that was exported.
        public Note Note { get; set; } = null!;  // Navigation to the related Note.


        [Required]
        [StringLength(20, ErrorMessage = "Format name cannot exceed 20 characters.")]
        public string Format { get; set; } = string.Empty;  // The export format used (e.g., PDF, PNG, JPEG).
        public DateTime ExportedAt { get; set; } = DateTime.UtcNow;  // The date and time when the export occurred.
        
        [StringLength(500)]
        public string? Destination { get; set; }  // Optional destination - e.g., file path or cloud target.
    }
}

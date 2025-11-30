using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    // Class Summary:
    /// Many-to-many join table linking Notes and Tags.
    /// A note can have many tags, and a tag can belong to many notes.
    public class NoteTag
    {

        [Required]
        public int NoteId { get; set; }  // Foreign key linking to the Note entity.

        [Required]
        public Note Note { get; set; } = null!;  /// Navigation property to the related Note.

        [Required]
        public int TagId { get; set; }  /// Foreign key linking to the Tag entity.

        [Required]
        public Tag Tag { get; set; } = null!;  /// Navigation property to the related Tag.
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    // Class Summary
    // Represents a category or label that can be attached to notes.
    // One tag can be assigned to many notes.
    public class Tag
    {
        /// Primary key for the Tag entity.
        [Key]
        public int Id { get; set; }

        /// Display name of the tag (e.g., "Math", "Exam", "Personal").
        [Required(ErrorMessage = "Tag name is required.")]
        [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// Many-to-many link to notes via NoteTag.
        public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
    }
}

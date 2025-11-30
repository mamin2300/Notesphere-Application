using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    // Class Summary:
    // Represents a reusable notebook page style (lined, dotted, grid, etc.)
    // Each Note references one NoteTemplate.

    public class NoteTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Template name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]  // Display name shown to the user (e.g., "Lined", "Dotted", "Grid").
        public string Name { get; set; } = string.Empty;

        [StringLength(120, ErrorMessage = "Description cannot exceed 120 characters.")]
        public string? Description { get; set; }  // description like "Ruled lines" or "Dot grid".
                                                  
        [Required(ErrorMessage = "CSS key is required.")]
        [StringLength(40)]
        public string CssKey { get; set; } = string.Empty; // A key that maps to a CSS class (lined, dotted, boxed).
                                               
        public string DefaultContent { get; set; } = string.Empty;  // default content loaded when a note is created using this template.                                                          
        public bool IsSystemTemplate { get; set; } = true;  // System templates (Lined, Dotted, Grid) cannot be deleted by users.
        public ICollection<Note> Notes { get; set; } = new List<Note>();  // Relationship: One Template → Many Notes.
    }
}

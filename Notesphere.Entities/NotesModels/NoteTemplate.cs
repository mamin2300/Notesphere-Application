using System.ComponentModel.DataAnnotations;
namespace Notesphere.Entities.NotesModels
{
    public class NoteTemplate
    {
        [Key]
        public int Id { get; set; }

        // Display name shown to user
        public string Name { get; set; } = string.Empty;

        // Short description like "Ruled lines", "Dot grid"
        public string? Description { get; set; }

        // Used to map to a CSS class: "lined", "dotgrid", etc.
        public string CssKey { get; set; } = string.Empty;

        // Optional: default text heading / structure
        public string DefaultContent { get; set; } = string.Empty;

        public bool IsSystemTemplate { get; set; } = true;
    }
}

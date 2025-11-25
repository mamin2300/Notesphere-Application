using System.ComponentModel.DataAnnotations;
namespace Notesphere.Entities.NotesModels
{
    public class NoteTemplate
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool isSystemTemplate { get; set; }
    }
}

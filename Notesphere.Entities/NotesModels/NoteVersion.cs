using System.ComponentModel.DataAnnotations;
namespace Notesphere.Entities.NotesModels
{
    public class NoteVersion
    {
        [Key]
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int VersionNumber { get; set; }
        public DateTime SavedAt { get; set; }
    }
}

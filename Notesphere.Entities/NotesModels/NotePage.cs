using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    public class NotePage
    {
        [Key]
        public int Id { get; set; }

        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int PageNumber { get; set; }

        public string ImageData { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        public int StudentUserId { get; set; }
        public StudentUser? StudentUser { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
        public int?TemplateId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



    }
}

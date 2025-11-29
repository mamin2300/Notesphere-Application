using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    public class StudentUser
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Simple password storage for assignment (hashed, not plain text)
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}

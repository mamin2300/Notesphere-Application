using System.ComponentModel.DataAnnotations;
namespace Notesphere.Entities.NotesModels
{
    public class StudentUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

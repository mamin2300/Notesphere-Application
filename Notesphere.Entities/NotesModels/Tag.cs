using System.ComponentModel.DataAnnotations;
namespace Notesphere.Entities.NotesModels
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

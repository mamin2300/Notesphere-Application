using System.ComponentModel.DataAnnotations;
namespace Notesphere.Entities.NotesModels
{
    public class NoteExport
    {
        [Key]
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Format { get; set; } = string.Empty;
        public DateTime ExportedAt { get; set; }
        public string? Destination { get; set; }

    }
}

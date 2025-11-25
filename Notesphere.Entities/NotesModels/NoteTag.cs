using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    public class NoteTag
    {
        [Key]
        public int NoteId { get; set; }
        public int TagId { get; set; }
    }
}

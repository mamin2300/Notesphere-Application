using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.DashboardModels
{
    public class Reminder
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime ReminderDate { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}

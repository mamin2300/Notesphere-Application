using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.DashboardModels
{
    public class QuickActions
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; } = string.Empty;

        public string TargetRoute { get; set; } = string.Empty; // control for when the user clicks quick action

        public string? Icon { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}

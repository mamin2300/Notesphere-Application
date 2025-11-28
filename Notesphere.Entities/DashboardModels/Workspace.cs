using System.ComponentModel.DataAnnotations;


namespace Notesphere.Entities.DashboardModels
{
    public class Workspace
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }
}

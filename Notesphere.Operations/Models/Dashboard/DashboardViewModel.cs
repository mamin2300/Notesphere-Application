using Notesphere.Entities.DashboardModels;
using Notesphere.Entities.NotesModels;
namespace Notesphere.Operations.Models.Dashboard
{
    public class DashboardViewModel
    {
        public DateTime Today { get; set; } = DateTime.Now;

        //Notes 
        public List<Note> RecentNotes { get; set; } = new();
        public List<Note> FavoriteNotes { get; set; } = new();
        public int TotalNotes   { get; set; }
        public int FavoriteNotesCount { get; set; } 

        //Dashboard Widgets
        public List<Reminder> Reminders { get; set; } = new();
        public List<QuickActions> QuickActions { get; set; } = new();
        public List<Workspace> Workspaces { get; set; } = new();
        public int?SelectedWorkspaceId { get; set; }



    }

}

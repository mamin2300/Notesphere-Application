using Notesphere.Entities.DashboardModels;

namespace Notesphere.Services.DashboardRepository
{
    public interface IDashboardServices
    {
        Task<List<Reminder>> GetRemindersAsync(DateTime from, DateTime to);
        Task<List<Reminder>> GetAllRemindersAsync();
        Task<Reminder?> GetReminderByIdAsync(int id);
        Task AddReminderAsync(Reminder reminder);
        Task UpdateReminderAsync(Reminder reminder);
        Task DeleteReminderAsync(int id);

        Task<List<Workspace>> GetWorkspacesAsync();
        Task<Workspace?> GetWorkspaceByIdAsync(int id);
        Task<List<QuickActions>> GetQuickActionsAsync();
    }
}

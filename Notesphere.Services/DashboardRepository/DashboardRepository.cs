using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.DashboardModels;
using Notesphere.Services.NotesphereDataAccessLayer;

namespace Notesphere.Services.DashboardRepository
{
    public class DashboardRepository: IDashboardServices
    {
        private readonly NotesphereDbContext _dbContext;
        public DashboardRepository(NotesphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }
       public async Task<List<Reminder>> GetRemindersAsync(DateTime from, DateTime to)
        {
            return await _dbContext.Reminders
            .Where(r=> !r.IsCompleted && r.CreatedAt >= from && r.CreatedAt <= to)
            .OrderBy(r => r.CreatedAt)
             .ToListAsync();
        }
        public async Task<List<Reminder>> GetAllRemindersAsync()
        {
            return await _dbContext.Reminders
            .OrderBy(r => r.CreatedAt)
             .ToListAsync();
        }

        public async Task<Reminder?> GetReminderByIdAsync(int id)
        {
            return await _dbContext.Reminders.FindAsync(id);
        }
        public async Task AddReminderAsync(Reminder reminder)
        {
            _dbContext.Reminders.Add(reminder);
            await  _dbContext.SaveChangesAsync();
        }
        public async Task UpdateReminderAsync(Reminder reminder)
        {
            _dbContext.Reminders.Update(reminder);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteReminderAsync(int id)
        {
            var reminder = await _dbContext.Reminders.FindAsync(id);
            if (reminder != null)
            {
                _dbContext.Reminders.Remove(reminder);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<Workspace>> GetWorkspacesAsync()
        {
            return await _dbContext.Workspaces
            .OrderBy(w => w.Name)
             .ToListAsync();
        }
        public async Task<Workspace?> GetWorkspaceByIdAsync(int id)
        {
            return await _dbContext.Workspaces.FirstOrDefaultAsync(w=>w.Id == id);
        }
        public async Task<List<QuickActions>> GetQuickActionsAsync()
        {
            return await _dbContext.QuickActions
            .Where(q => q.IsEnabled)
             .OrderByDescending(q=> q.IsPrimary)
                .ToListAsync();
        }


    }
}

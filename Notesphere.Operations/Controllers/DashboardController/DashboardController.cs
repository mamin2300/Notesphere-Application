using Microsoft.AspNetCore.Mvc;
using Notesphere.Operations.Models;
using Notesphere.Operations.Models.Dashboard;
using Notesphere.Services.DashboardRepository;
using Notesphere.Services.NotesRepository;

namespace Notesphere.Operations.Controllers.DashboardController
{
    public class DashboardController : Controller
    {
        private readonly INotesService _notesRepo;
        private readonly IDashboardServices _dashRepo;

        public DashboardController(INotesService notesRepo, IDashboardServices dashRepo)
        {
            _notesRepo = notesRepo;
            _dashRepo = dashRepo;
        }
        public async Task<IActionResult> Index(int? workspaceId)
        {
            var notes = await _notesRepo.GetAllNotes();

            var recentNotes = notes
                .OrderByDescending(n => n.UpdatedAt)
                .Take(3)
                .ToList();

            var favoriteNotes = notes
                .Where(n => n.IsFavorite)
                .OrderByDescending(n => n.UpdatedAt)
                .Take(3)
                .ToList();
            var workspaces = await _dashRepo.GetWorkspacesAsync();
            var selectedWorkspaceId = workspaceId ?? workspaces.FirstOrDefault()?.Id;

            var today = DateTime.Today;
            var weekAhead = today.AddDays(7);

            var reminders = await _dashRepo.GetRemindersAsync(today, weekAhead);
            var quickActions = await _dashRepo.GetQuickActionsAsync();

            var vm = new DashboardViewModel
            {
                Today = today,
                RecentNotes = recentNotes,
                FavoriteNotes = favoriteNotes,
                TotalNotes = notes.Count,
                FavoriteNotesCount = favoriteNotes.Count,
                Reminders = reminders,
                QuickActions = quickActions,
                Workspaces = workspaces,
                SelectedWorkspaceId = selectedWorkspaceId
            };

            return View(vm);

        }
    }
}

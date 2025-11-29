using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.PlannerModels;
using Notesphere.Operations.Models.Planner;
using Notesphere.Operations.PlannerServices;
using Notesphere.Services.NotesphereDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notesphere.Operations.Controllers
{
    [Authorize]
    public class PlannerController : Controller
    {
        private readonly PlannerService _plannerService;

        public PlannerController(PlannerService plannerService)
        {
            _plannerService = plannerService;
        }

        // GET: /Planner - Main weekly planner view
        public async Task<IActionResult> Index(DateTime? date)
        {
            string userId = GetCurrentUserId();

            // Default to current week if no date specified
            DateTime targetDate = date ?? DateTime.Today;
            DateTime weekStart = GetWeekStartDate(targetDate);

            var viewModel = new WeeklyPlannerViewModel
            {
                WeekStartDate = weekStart,
                WeekEndDate = weekStart.AddDays(6),
                PreviousWeek = weekStart.AddDays(-7),
                NextWeek = weekStart.AddDays(7)
            };

            // Get events for this week
            viewModel.Events = await _plannerService.GetWeeklyPlannerAsync(userId, weekStart);

            // Organize events by day
            foreach (var evt in viewModel.Events)
            {
                DayOfWeek day = evt.StartTime.DayOfWeek;
                if (viewModel.EventsByDay.ContainsKey(day))
                {
                    viewModel.EventsByDay[day].Add(evt);
                }
            }

            // Get upcoming events
            viewModel.UpcomingEvents = await _plannerService.GetUpcomingEventsAsync(userId, 7);

            // Get unresolved conflicts
            viewModel.Conflicts = await _plannerService.GetUnresolvedConflictsAsync(userId);
            viewModel.HasConflicts = viewModel.Conflicts.Any();

            return View(viewModel);
        }

        // GET: /Planner/CreateEvent
        public IActionResult CreateEvent(DateTime? date)
        {
            var viewModel = new EventFormViewModel();

            // Pre-fill date if provided
            if (date.HasValue)
            {
                viewModel.StartTime = date.Value.AddHours(9);
                viewModel.EndTime = date.Value.AddHours(10);
            }

            return View(viewModel);
        }

        // POST: /Planner/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(EventFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            string userId = GetCurrentUserId();

            try
            {
                // Validate end time is after start time
                if (viewModel.EndTime <= viewModel.StartTime)
                {
                    ModelState.AddModelError("EndTime", "End time must be after start time");
                    return View(viewModel);
                }

                Event newEvent = viewModel.ToEvent(userId);

                // Check if recurring
                if (viewModel.IsRecurring)
                {
                    RecurringEvent recurrence = viewModel.ToRecurringEvent();
                    await _plannerService.CreateRecurringEventAsync(newEvent, recurrence, userId);
                    TempData["SuccessMessage"] = "Recurring event created successfully!";
                }
                else
                {
                    await _plannerService.CreateEventAsync(newEvent, userId);
                    TempData["SuccessMessage"] = "Event created successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");
                return View(viewModel);
            }
        }

        // GET: /Planner/EditEvent/5
        public async Task<IActionResult> EditEvent(int id)
        {
            string userId = GetCurrentUserId();

            var evt = await _plannerService.GetEventByIdAsync(id, userId);
            if (evt == null)
                return NotFound();

            var viewModel = EventFormViewModel.FromEvent(evt);
            return View(viewModel);
        }

        // POST: /Planner/EditEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, EventFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            string userId = GetCurrentUserId();

            try
            {
                // Validate end time
                if (viewModel.EndTime <= viewModel.StartTime)
                {
                    ModelState.AddModelError("EndTime", "End time must be after start time");
                    return View(viewModel);
                }

                Event updatedEvent = viewModel.ToEvent(userId);
                bool success = await _plannerService.UpdateEventAsync(id, updatedEvent, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Event not found or you don't have permission to edit it");
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating event: {ex.Message}");
                return View(viewModel);
            }
        }

        // POST: /Planner/DeleteEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            string userId = GetCurrentUserId();

            try
            {
                bool success = await _plannerService.DeleteEventAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Event deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found or you don't have permission to delete it";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting event: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Planner/Conflicts
        public async Task<IActionResult> Conflicts(bool showResolved = false)
        {
            string userId = GetCurrentUserId();

            var viewModel = new ConflictViewModel
            {
                UserId = userId,
                ShowResolved = showResolved,
                ReportStartDate = DateTime.Today,
                ReportEndDate = DateTime.Today.AddMonths(1)
            };

            // Get conflicts
            if (showResolved)
            {
                viewModel.Conflicts = await _plannerService.DetectAllConflictsAsync(userId);
            }
            else
            {
                viewModel.UnresolvedConflicts = await _plannerService.GetUnresolvedConflictsAsync(userId);
                viewModel.Conflicts = viewModel.UnresolvedConflicts;
            }

            // Calculate statistics
            viewModel.CalculateStatistics();

            return View(viewModel);
        }

        // POST: /Planner/ResolveConflict/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveConflict(int id)
        {
            try
            {
                bool success = await _plannerService.ResolveConflictAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Conflict marked as resolved!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Conflict not found";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error resolving conflict: {ex.Message}";
            }

            return RedirectToAction(nameof(Conflicts));
        }

        // GET: /Planner/GetEventsJson - For AJAX calendar updates
        [HttpGet]
        public async Task<IActionResult> GetEventsJson(DateTime start, DateTime end)
        {
            string userId = GetCurrentUserId();

            var events = await _plannerService.GetEventsByDateRangeAsync(userId, start, end);

            // Format for FullCalendar or similar JS library
            var eventData = events.Select(e => new
            {
                id = e.EventId,
                title = e.Title,
                start = e.StartTime,
                end = e.EndTime,
                color = e.ColorCode,
                allDay = e.IsAllDay
            });

            return Json(eventData);
        }

        // Helper methods

        // Gets current logged-in user ID
        private string GetCurrentUserId()
        {
            // Get user ID from claims (ASP.NET Identity)
            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? "default-user"; // Fallback for testing
        }

        // Gets the Monday of the week for a given date
        private DateTime GetWeekStartDate(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}

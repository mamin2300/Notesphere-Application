using Notesphere.Entities.PlannerModels;
using System.ComponentModel.DataAnnotations;

namespace Notesphere.Operations.Models.Planner
{
    public class EventFormViewModel
    {
        // Event details
        public int? EventId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        public string ColorCode { get; set; }

        [Required(ErrorMessage = "Event type is required")]
        public string EventType { get; set; }

        public bool IsAllDay { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        // Recurrence settings
        public bool IsRecurring { get; set; }
        public string RecurrencePattern { get; set; } // Daily, Weekly, Monthly
        public int RecurrenceInterval { get; set; }
        public string RecurrenceDaysOfWeek { get; set; } // For weekly: "Monday,Wednesday,Friday"
        public DateTime? RecurrenceEndDate { get; set; }
        public int? MaxOccurrences { get; set; }

        // Available options for dropdowns
        public List<string> ColorOptions { get; set; }
        public List<string> EventTypeOptions { get; set; }
        public List<string> RecurrencePatternOptions { get; set; }

        // Conflict warnings
        public List<Conflict> PotentialConflicts { get; set; }
        public bool HasConflicts { get; set; }

        // Form state
        public bool IsEditMode { get; set; }

        // Constructor
        public EventFormViewModel()
        {
            // Default values
            StartTime = DateTime.Today.AddHours(9); // 9 AM
            EndTime = DateTime.Today.AddHours(10);   // 10 AM
            ColorCode = "#49111C"; // Default burgundy
            IsAllDay = false;
            IsRecurring = false;
            RecurrenceInterval = 1;

            // Color options (project color scheme)
            ColorOptions = new List<string>
            {
                "#0A0908", // Black
                "#49111C", // Burgundy
                "#A9927D", // Tan
                "#5E503F", // Olive
                "#3B82F6", // Blue
                "#10B981", // Green
                "#F59E0B", // Orange
                "#EF4444"  // Red
            };

            // Event types
            EventTypeOptions = new List<string>
            {
                "Class",
                "Assignment",
                "Exam",
                "Lab",
                "Tutorial",
                "Meeting",
                "Study",
                "Personal"
            };

            // Recurrence patterns
            RecurrencePatternOptions = new List<string>
            {
                "Daily",
                "Weekly",
                "Monthly"
            };

            PotentialConflicts = new List<Conflict>();
            HasConflicts = false;
        }

        // Convert to Event entity
        public Event ToEvent(string userId)
        {
            return new Event
            {
                EventId = EventId ?? 0,
                UserId = userId,
                Title = Title,
                Description = Description,
                StartTime = StartTime,
                EndTime = EndTime,
                Location = Location,
                ColorCode = ColorCode,
                EventType = EventType,
                IsAllDay = IsAllDay,
                Notes = Notes
            };
        }

        // Convert to RecurringEvent entity
        public RecurringEvent ToRecurringEvent()
        {
            if (!IsRecurring)
                return null;

            return new RecurringEvent
            {
                Pattern = RecurrencePattern,
                Interval = RecurrenceInterval,
                DaysOfWeek = RecurrenceDaysOfWeek,
                RecurrenceStartDate = StartTime.Date,
                RecurrenceEndDate = RecurrenceEndDate,
                MaxOccurrences = MaxOccurrences,
                IsActive = true
            };
        }

        // Load from Event entity
        public static EventFormViewModel FromEvent(Event evt)
        {
            var viewModel = new EventFormViewModel
            {
                EventId = evt.EventId,
                Title = evt.Title,
                Description = evt.Description,
                StartTime = evt.StartTime,
                EndTime = evt.EndTime,
                Location = evt.Location,
                ColorCode = evt.ColorCode,
                EventType = evt.EventType,
                IsAllDay = evt.IsAllDay,
                Notes = evt.Notes,
                IsEditMode = true
            };

            // Load recurrence info if exists
            if (evt.RecurringEvent != null)
            {
                viewModel.IsRecurring = true;
                viewModel.RecurrencePattern = evt.RecurringEvent.Pattern;
                viewModel.RecurrenceInterval = evt.RecurringEvent.Interval;
                viewModel.RecurrenceDaysOfWeek = evt.RecurringEvent.DaysOfWeek;
                viewModel.RecurrenceEndDate = evt.RecurringEvent.RecurrenceEndDate;
                viewModel.MaxOccurrences = evt.RecurringEvent.MaxOccurrences;
            }

            return viewModel;
        }
    }
}

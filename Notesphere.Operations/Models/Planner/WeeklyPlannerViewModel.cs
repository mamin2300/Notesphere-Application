using Notesphere.Entities.PlannerModels;

namespace Notesphere.Operations.Models.Planner
{
    public class WeeklyPlannerViewModel
    {
        // Current week being displayed
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }

        // All events for this week
        public List<Event> Events { get; set; }

        // Events organized by day
        public Dictionary<DayOfWeek, List<Event>> EventsByDay { get; set; }

        // Conflicts for this week
        public List<Conflict> Conflicts { get; set; }
        public bool HasConflicts { get; set; }

        // Upcoming events (next 7 days)
        public List<Event> UpcomingEvents { get; set; }

        // Color options for events (using project color scheme)
        public List<string> ColorOptions { get; set; }

        // Available event types
        public List<string> EventTypes { get; set; }

        // Current date for highlighting
        public DateTime Today { get; set; }

        // Week navigation
        public DateTime PreviousWeek { get; set; }
        public DateTime NextWeek { get; set; }

        // Constructor
        public WeeklyPlannerViewModel()
        {
            Events = new List<Event>();
            EventsByDay = new Dictionary<DayOfWeek, List<Event>>();
            Conflicts = new List<Conflict>();
            UpcomingEvents = new List<Event>();
            Today = DateTime.Today;

            // Project color scheme
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
            EventTypes = new List<string>
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

            InitializeEventsByDay();
        }

        // Initialize empty lists for each day
        private void InitializeEventsByDay()
        {
            EventsByDay[DayOfWeek.Monday] = new List<Event>();
            EventsByDay[DayOfWeek.Tuesday] = new List<Event>();
            EventsByDay[DayOfWeek.Wednesday] = new List<Event>();
            EventsByDay[DayOfWeek.Thursday] = new List<Event>();
            EventsByDay[DayOfWeek.Friday] = new List<Event>();
            EventsByDay[DayOfWeek.Saturday] = new List<Event>();
            EventsByDay[DayOfWeek.Sunday] = new List<Event>();
        }

        // Get formatted week display "Nov 18 - Nov 24"
        public string GetWeekDisplay()
        {
            return $"{WeekStartDate:MMM dd} - {WeekEndDate:MMM dd}";
        }

        // Check if a specific day is today
        public bool IsToday(DayOfWeek day)
        {
            return Today.DayOfWeek == day;
        }
    }
}

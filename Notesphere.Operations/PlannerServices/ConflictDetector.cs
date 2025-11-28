using Notesphere.Entities.PlannerModels;

namespace Notesphere.Operations.PlannerServices
{
    
    // Detects and analyzes scheduling conflicts between events.
   
    public class ConflictDetector
    {
        private readonly TimeSpan _minimumBufferTime;

        
        // Constructor - sets default buffer time between events
        public ConflictDetector()
        {
            // Default 15 minute buffer between events
            _minimumBufferTime = TimeSpan.FromMinutes(15);
        }

       
        // Detects all conflicts within a list of events
        public List<Conflict> DetectConflicts(List<Event> events)
        {
            var conflicts = new List<Conflict>();

            if (events == null || events.Count < 2)
                return conflicts;

            // Compare each event with every other event
            for (int i = 0; i < events.Count - 1; i++)
            {
                for (int j = i + 1; j < events.Count; j++)
                {
                    var event1 = events[i];
                    var event2 = events[j];

                    // Check if these two events conflict
                    if (HasConflict(event1, event2))
                    {
                        var conflict = Conflict.CreateFromEvents(event1, event2);
                        if (conflict != null)
                        {
                            conflicts.Add(conflict);
                        }
                    }
                }
            }

            return conflicts;
        }

       
        // Checks if a new event conflicts with existing events
        public List<Conflict> CheckNewEventConflicts(Event newEvent, List<Event> existingEvents)
        {
            var conflicts = new List<Conflict>();

            if (newEvent == null || existingEvents == null || !existingEvents.Any())
                return conflicts;

            foreach (var existingEvent in existingEvents)
            {
                // Skip comparing event with itself
                if (existingEvent.EventId == newEvent.EventId)
                    continue;

                if (HasConflict(newEvent, existingEvent))
                {
                    var conflict = Conflict.CreateFromEvents(newEvent, existingEvent);
                    if (conflict != null)
                    {
                        conflicts.Add(conflict);
                    }
                }
            }

            return conflicts;
        }

        
        // Checks if two events have a time conflict
        public bool HasConflict(Event event1, Event event2)
        {
            if (event1 == null || event2 == null)
                return false;

            // Events must be on the same day to conflict
            if (event1.StartTime.Date != event2.StartTime.Date)
                return false;

            // Check for time overlap
            return CheckTimeOverlap(
                event1.StartTime,
                event1.EndTime,
                event2.StartTime,
                event2.EndTime);
        }

       
        // Finds all events that conflict with a given event
        public List<Event> FindConflictingEvents(Event targetEvent, List<Event> allEvents)
        {
            var conflictingEvents = new List<Event>();

            if (targetEvent == null || allEvents == null)
                return conflictingEvents;

            foreach (var evt in allEvents)
            {
                // Skip the target event itself
                if (evt.EventId == targetEvent.EventId)
                    continue;

                if (HasConflict(targetEvent, evt))
                {
                    conflictingEvents.Add(evt);
                }
            }

            return conflictingEvents;
        }

       
        // Generates suggestions for resolving conflicts
        public string[] GenerateConflictSuggestions(List<Conflict> conflicts)
        {
            var suggestions = new List<string>();

            if (conflicts == null || !conflicts.Any())
            {
                suggestions.Add("No conflicts detected. Your schedule is clear!");
                return suggestions.ToArray();
            }

            suggestions.Add($"You have {conflicts.Count} scheduling conflict(s).");
            suggestions.Add("Consider the following options:");
            suggestions.Add("• Reschedule one of the conflicting events");
            suggestions.Add("• Reduce the duration of overlapping events");
            suggestions.Add("• Cancel or delegate one of the events");
            suggestions.Add("• Add buffer time between back-to-back events");

            // Add specific suggestions for severe conflicts
            var severeConflicts = conflicts.Where(c => c.ConflictSeverity == "Severe").ToList();
            if (severeConflicts.Any())
            {
                suggestions.Add($"⚠️ {severeConflicts.Count} severe conflict(s) need immediate attention!");
            }

            return suggestions.ToArray();
        }

       
        // Suggests alternative time slots for an event to avoid conflicts
        public DateTime? SuggestAlternativeTime(Event targetEvent, List<Event> existingEvents)
        {
            if (targetEvent == null)
                return null;

            TimeSpan eventDuration = targetEvent.GetDuration();
            DateTime currentDate = targetEvent.StartTime.Date;
            DateTime searchStart = currentDate.AddHours(8); // Start searching from 8 AM
            DateTime searchEnd = currentDate.AddHours(22); // Search until 10 PM

            // Try to find a free slot
            DateTime proposedStart = searchStart;

            while (proposedStart.AddMinutes(eventDuration.TotalMinutes) <= searchEnd)
            {
                DateTime proposedEnd = proposedStart.Add(eventDuration);

                // Check if this slot conflicts with any existing event
                bool hasConflict = existingEvents.Any(e =>
                    CheckTimeOverlap(proposedStart, proposedEnd, e.StartTime, e.EndTime));

                if (!hasConflict)
                {
                    return proposedStart; // Found a free slot!
                }

                // Move to next 30-minute slot
                proposedStart = proposedStart.AddMinutes(30);
            }

            // No free slot found on this day
            return null;
        }

        
        // Gets a summary report of conflicts
        public string GetConflictSummary(List<Conflict> conflicts)
        {
            if (conflicts == null || !conflicts.Any())
                return "No scheduling conflicts detected.";

            int minorCount = conflicts.Count(c => c.ConflictSeverity == "Minor");
            int moderateCount = conflicts.Count(c => c.ConflictSeverity == "Moderate");
            int severeCount = conflicts.Count(c => c.ConflictSeverity == "Severe");

            return $"Total Conflicts: {conflicts.Count}\n" +
                   $"• Minor: {minorCount}\n" +
                   $"• Moderate: {moderateCount}\n" +
                   $"• Severe: {severeCount}";
        }

        // Private Helper Methods

        
        // Checks if two time ranges overlap
        private bool CheckTimeOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            // Two time ranges overlap if:
            // start1 < end2 AND end1 > start2
            return start1 < end2 && end1 > start2;
        }

        
        // Calculates the overlap duration between two time rangess
        private TimeSpan CalculateOverlapDuration(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            // Find the later start time
            DateTime overlapStart = start1 > start2 ? start1 : start2;

            // Find the earlier end time
            DateTime overlapEnd = end1 < end2 ? end1 : end2;

            // Calculate overlap
            if (overlapEnd > overlapStart)
            {
                return overlapEnd - overlapStart;
            }

            return TimeSpan.Zero;
        }


        // Determines severity based on overlap duration and event types

        private string DetermineConflictSeverity(TimeSpan overlapDuration, string eventType1, string eventType2)
        {
            int overlapMinutes = (int)overlapDuration.TotalMinutes;

            // Severe: 1 hour or more overlap, or involves exams
            if (overlapMinutes >= 60 ||
                eventType1 == "Exam" || eventType2 == "Exam")
            {
                return "Severe";
            }

            // Moderate: 30-60 minutes overlap, or involves classes
            if (overlapMinutes >= 30 ||
                eventType1 == "Class" || eventType2 == "Class")
            {
                return "Moderate";
            }

            // Minor: less than 30 minutes
            return "Minor";
        }

       
        // Checks if events have adequate buffer time between them
        private bool HasAdequateBuffer(Event event1, Event event2)
        {
            // If event1 ends before event2 starts
            if (event1.EndTime <= event2.StartTime)
            {
                TimeSpan buffer = event2.StartTime - event1.EndTime;
                return buffer >= _minimumBufferTime;
            }

            // If event2 ends before event1 starts
            if (event2.EndTime <= event1.StartTime)
            {
                TimeSpan buffer = event1.StartTime - event2.EndTime;
                return buffer >= _minimumBufferTime;
            }

            // Events overlap - no buffer
            return false;
        }
    }
}

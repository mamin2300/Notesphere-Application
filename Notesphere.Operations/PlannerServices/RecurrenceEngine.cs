using Notesphere.Entities.PlannerModels;

namespace Notesphere.Operations.PlannerServices
{
    public class RecurrenceEngine
    {

        // Generates individual event occurrences from a recurring pattern
        public List<Event> GenerateOccurrences(Event baseEvent, RecurringEvent recurrence, DateTime startDate, DateTime endDate)
        {
            var occurrences = new List<Event>();

            if (!recurrence.IsActive || !recurrence.ValidateRecurrence())
                return occurrences;

            DateTime currentDate = recurrence.RecurrenceStartDate > startDate
                ? recurrence.RecurrenceStartDate
                : startDate;

            DateTime limitDate = recurrence.RecurrenceEndDate.HasValue && recurrence.RecurrenceEndDate.Value < endDate
                ? recurrence.RecurrenceEndDate.Value
                : endDate;

            int count = 0;

            // Generate occurrences based on pattern
            while (currentDate <= limitDate)
            {
                // Check max occurrences limit
                if (recurrence.MaxOccurrences.HasValue && count >= recurrence.MaxOccurrences.Value)
                    break;

                // Skip exception dates
                if (!IsExceptionDate(currentDate, recurrence.ExceptionDates))
                {
                    // Check if this date matches the pattern
                    if (IsValidOccurrenceDate(currentDate, recurrence))
                    {
                        // Create a new event instance for this occurrence
                        var occurrence = baseEvent.Clone();

                        // Calculate the time difference to apply to the occurrence
                        TimeSpan timeDiff = currentDate - baseEvent.StartTime.Date;
                        occurrence.StartTime = baseEvent.StartTime.Add(timeDiff);
                        occurrence.EndTime = baseEvent.EndTime.Add(timeDiff);
                        occurrence.RecurrenceId = recurrence.RecurrenceId;

                        occurrences.Add(occurrence);
                        count++;
                    }
                }

                // Move to next date
                currentDate = GetNextDate(currentDate, recurrence);

                // Safety limit to prevent infinite loops
                if (currentDate.Year > DateTime.Now.Year + 10)
                    break;
            }

            return occurrences;
        }

        // Expands recurring events in a list for a specific date range
        public List<Event> ExpandRecurringEvents(List<Event> events, DateTime startDate, DateTime endDate)
        {
            var expandedEvents = new List<Event>();

            foreach (var evt in events)
            {
                // If event has no recurrence, just add it
                if (evt.RecurrenceId == null || evt.RecurringEvent == null)
                {
                    expandedEvents.Add(evt);
                }
                else
                {
                    // Generate all occurrences for this recurring event
                    var occurrences = GenerateOccurrences(evt, evt.RecurringEvent, startDate, endDate);
                    expandedEvents.AddRange(occurrences);
                }
            }

            return expandedEvents;
        }

        // Validates if a recurrence pattern is properly configured
        public bool ValidateRecurrencePattern(RecurringEvent recurrence)
        {
            if (recurrence == null)
                return false;

            if (string.IsNullOrWhiteSpace(recurrence.Pattern))
                return false;

            if (recurrence.Interval <= 0)
                return false;

            // Weekly pattern must have days specified
            if (recurrence.Pattern.Equals("Weekly", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(recurrence.DaysOfWeek))
                return false;

            // End date validation
            if (recurrence.RecurrenceEndDate.HasValue
                && recurrence.RecurrenceEndDate.Value <= recurrence.RecurrenceStartDate)
                return false;

            return true;
        }

        // Calculates the next occurrence date after a given date
        public DateTime? CalculateNextOccurrence(RecurringEvent recurrence, DateTime afterDate)
        {
            if (!recurrence.IsActive)
                return null;

            DateTime nextDate = afterDate.AddDays(1);

            // Check if past end date
            if (recurrence.RecurrenceEndDate.HasValue && nextDate > recurrence.RecurrenceEndDate.Value)
                return null;

            // Find next valid date
            int attempts = 0;
            while (nextDate <= (recurrence.RecurrenceEndDate ?? DateTime.MaxValue))
            {
                if (IsValidOccurrenceDate(nextDate, recurrence)
                    && !IsExceptionDate(nextDate, recurrence.ExceptionDates))
                {
                    return nextDate;
                }

                nextDate = GetNextDate(nextDate, recurrence);

                // Prevent infinite loop
                attempts++;
                if (attempts > 3650) // Max ~10 years
                    break;
            }

            return null;
        }

        // Private helper methods

        // Checks if a date is in the exception list
        private bool IsExceptionDate(DateTime date, string exceptionDates)
        {
            if (string.IsNullOrEmpty(exceptionDates))
                return false;

            return exceptionDates.Contains(date.ToString("yyyy-MM-dd"));
        }

        // Checks if a date matches the recurrence pattern
        private bool IsValidOccurrenceDate(DateTime date, RecurringEvent recurrence)
        {
            switch (recurrence.Pattern.ToLower())
            {
                case "daily":
                    return true; // Every day is valid

                case "weekly":
                    // Check if this day of week is in the list
                    if (string.IsNullOrEmpty(recurrence.DaysOfWeek))
                        return false;
                    return recurrence.DaysOfWeek.Contains(date.DayOfWeek.ToString());

                case "monthly":
                    // Same day of month as start date
                    return date.Day == recurrence.RecurrenceStartDate.Day;

                case "yearly":
                    // Same month and day as start date
                    return date.Month == recurrence.RecurrenceStartDate.Month
                        && date.Day == recurrence.RecurrenceStartDate.Day;

                default:
                    return false;
            }
        }

        // Gets the next date to check based on pattern
        private DateTime GetNextDate(DateTime currentDate, RecurringEvent recurrence)
        {
            switch (recurrence.Pattern.ToLower())
            {
                case "daily":
                    return currentDate.AddDays(recurrence.Interval);

                case "weekly":
                    return currentDate.AddDays(1); // Check each day for weekly patterns

                case "monthly":
                    return currentDate.AddMonths(recurrence.Interval);

                case "yearly":
                    return currentDate.AddYears(recurrence.Interval);

                default:
                    return currentDate.AddDays(1);
            }
        }

        // Applies daily recurrence pattern
        private List<DateTime> ApplyDailyRecurrence(DateTime baseDate, int interval, DateTime endLimit)
        {
            var dates = new List<DateTime>();
            DateTime current = baseDate;

            while (current <= endLimit)
            {
                dates.Add(current);
                current = current.AddDays(interval);
            }

            return dates;
        }

        // Applies weekly recurrence pattern
        private List<DateTime> ApplyWeeklyRecurrence(DateTime baseDate, int interval, string daysOfWeek, DateTime endLimit)
        {
            var dates = new List<DateTime>();

            if (string.IsNullOrEmpty(daysOfWeek))
                return dates;

            DateTime current = baseDate;

            while (current <= endLimit)
            {
                // Check if current day matches any specified days
                if (daysOfWeek.Contains(current.DayOfWeek.ToString()))
                {
                    dates.Add(current);
                }

                current = current.AddDays(1);

                // Skip weeks based on interval
                if (current.DayOfWeek == baseDate.DayOfWeek && interval > 1)
                {
                    current = current.AddDays(7 * (interval - 1));
                }
            }

            return dates;
        }

        // Applies monthly recurrence pattern
        private List<DateTime> ApplyMonthlyRecurrence(DateTime baseDate, int interval, DateTime endLimit)
        {
            var dates = new List<DateTime>();
            DateTime current = baseDate;

            while (current <= endLimit)
            {
                dates.Add(current);
                current = current.AddMonths(interval);

                // Handle month-end edge cases
                if (current.Day != baseDate.Day)
                {
                    // If target day doesn't exist in month, use last day of month
                    int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);
                    if (baseDate.Day > daysInMonth)
                    {
                        current = new DateTime(current.Year, current.Month, daysInMonth);
                    }
                }
            }

            return dates;
        }
    }
}

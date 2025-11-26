using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Malika's : Showcases recurring events and marks it.
namespace Notesphere.Entities.PlannerModels
{
    
    /// Represents a recurring pattern for events (e.g., every Monday/Wednesday, daily, monthly).
    /// Used to generate multiple event occurrences from a single pattern.
    
    public class RecurringEvent
    {
        // Primary Key
        [Key]
        public int RecurrenceId { get; set; }

        // Foreign Key - links to the parent/template Event
        [Required]
        public int ParentEventId { get; set; }

        // Recurrence pattern: Daily, Weekly, Monthly, Custom
        [Required]
        [StringLength(50)]
        public string Pattern { get; set; }

        // Interval: repeat every X days/weeks/months (e.g., every 2 weeks)
        [Required]
        public int Interval { get; set; }

        // Days of week for weekly recurrence (comma-separated: "Monday,Wednesday,Friday")
        // Used when Pattern = "Weekly"
        [StringLength(100)]
        public string DaysOfWeek { get; set; }

        // Start date of recurrence
        [Required]
        public DateTime RecurrenceStartDate { get; set; }

        // End date of recurrence (nullable - can recur indefinitely)
        public DateTime? RecurrenceEndDate { get; set; }

        // Maximum number of occurrences (alternative to end date)
       
        public int? MaxOccurrences { get; set; }

        
        // Used for holidays or cancelled classes
        [StringLength(1000)]
        public string ExceptionDates { get; set; }

        
        public bool IsActive { get; set; }

        // Navigation property - collection of events generated from this pattern
        public virtual ICollection<Event> Events { get; set; }

        // Constructor 
        public RecurringEvent()
        {
            IsActive = true;
            Interval = 1; // Default: repeat every 1 week/day/month
            Events = new List<Event>();
        }

        
        // Generates all event occurrences for this recurring pattern within a date range
    
        public List<Event> GenerateOccurrences(DateTime startDate, DateTime endDate)
        {
            var occurrences = new List<Event>();

            // Don't generate if pattern is inactive
            if (!IsActive)
                return occurrences;

            DateTime currentDate = RecurrenceStartDate > startDate ? RecurrenceStartDate : startDate;
            DateTime limitDate = RecurrenceEndDate.HasValue && RecurrenceEndDate.Value < endDate
                ? RecurrenceEndDate.Value
                : endDate;

            int count = 0;

            // Generate occurrences based on pattern
            while (currentDate <= limitDate)
            {
                // Check if we've hit max occurrences limit
                if (MaxOccurrences.HasValue && count >= MaxOccurrences.Value)
                    break;

                // Check if this date is an exception (skip it)
                if (!IsExceptionDate(currentDate))
                {
                    // Check if this day matches the pattern
                    if (IsValidOccurrenceDate(currentDate))
                    {
                        // This would be handled by the service layer to create actual Event objects
                        count++;
                    }
                }

                // Move to next date based on pattern
                currentDate = GetNextDate(currentDate);
            }

            return occurrences;
        }

       
        // Adds a date to the exception list (date to skip)
     
        public void AddExceptionDate(DateTime date)
        {
            if (string.IsNullOrEmpty(ExceptionDates))
            {
                ExceptionDates = date.ToString("yyyy-MM-dd");
            }
            else
            {
                ExceptionDates += "," + date.ToString("yyyy-MM-dd");
            }
        }

       
        // Removes a date from the exception list
        public void RemoveExceptionDate(DateTime date)
        {
            if (string.IsNullOrEmpty(ExceptionDates))
                return;

            var dates = ExceptionDates.Split(',').ToList();
            dates.Remove(date.ToString("yyyy-MM-dd"));
            ExceptionDates = string.Join(",", dates);
        }

       
        // Gets the next occurrence date after a given date
        public DateTime? GetNextOccurrence(DateTime afterDate)
        {
            if (!IsActive)
                return null;

            DateTime nextDate = afterDate.AddDays(1);

            // If we have an end date and we're past it, no more occurrences
            if (RecurrenceEndDate.HasValue && nextDate > RecurrenceEndDate.Value)
                return null;

            // Find the next valid date based on pattern
            while (nextDate <= (RecurrenceEndDate ?? DateTime.MaxValue))
            {
                if (IsValidOccurrenceDate(nextDate) && !IsExceptionDate(nextDate))
                {
                    return nextDate;
                }

                nextDate = GetNextDate(nextDate);

                // Prevent infinite loop
                if (nextDate.Year > DateTime.Now.Year + 10)
                    break;
            }

            return null;
        }

        
        // Validates if the recurrence pattern is properly configured
      
        public bool ValidateRecurrence()
        {
            // Pattern must be specified
            if (string.IsNullOrWhiteSpace(Pattern))
                return false;

            // Interval must be positive
            if (Interval <= 0)
                return false;

            // If weekly pattern, must have days specified
            if (Pattern.Equals("Weekly", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(DaysOfWeek))
                return false;

            // End date must be after start date if specified
            if (RecurrenceEndDate.HasValue && RecurrenceEndDate.Value <= RecurrenceStartDate)
                return false;

            // Max occurrences must be positive if specified
            if (MaxOccurrences.HasValue && MaxOccurrences.Value <= 0)
                return false;

            return true;
        }

       
        // Calculates the end date based on max occurrences (if end date not set)
       
        public DateTime? CalculateEndDate()
        {
            if (RecurrenceEndDate.HasValue)
                return RecurrenceEndDate;

            if (!MaxOccurrences.HasValue)
                return null;

            DateTime calculatedEnd = RecurrenceStartDate;
            int count = 0;

            while (count < MaxOccurrences.Value)
            {
                if (IsValidOccurrenceDate(calculatedEnd))
                    count++;

                calculatedEnd = GetNextDate(calculatedEnd);

                // Safety limit
                if (calculatedEnd.Year > RecurrenceStartDate.Year + 10)
                    break;
            }

            return calculatedEnd;
        }

        // Private helper methods
        // Checks if a date is in the exception list
        
        private bool IsExceptionDate(DateTime date)
        {
            if (string.IsNullOrEmpty(ExceptionDates))
                return false;

            return ExceptionDates.Contains(date.ToString("yyyy-MM-dd"));
        }

        
        // Checks if a date matches the recurrence pattern
        
        private bool IsValidOccurrenceDate(DateTime date)
        {
            switch (Pattern.ToLower())
            {
                case "daily":
                    return true; // Every day is valid

                case "weekly":
                    // Check if this day of week is in our list
                    if (string.IsNullOrEmpty(DaysOfWeek))
                        return false;
                    return DaysOfWeek.Contains(date.DayOfWeek.ToString());

                case "monthly":
                    // Check if this is the same day of month as start date
                    return date.Day == RecurrenceStartDate.Day;

                default:
                    return false;
            }
        }

     
        /// Gets the next date to check based on pattern and interval
       
        private DateTime GetNextDate(DateTime currentDate)
        {
            switch (Pattern.ToLower())
            {
                case "daily":
                    return currentDate.AddDays(Interval);

                case "weekly":
                    return currentDate.AddDays(1); // Check next day for weekly

                case "monthly":
                    return currentDate.AddMonths(Interval);

                default:
                    return currentDate.AddDays(1);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Notesphere.Entities.PlannerModels
{
 
    // Represents a scheduling conflict between two events that overlap in time.
    // to detect and display conflicts to help students manage their schedule.
    
    public class Conflict
    {
        // Primary Key
        [Key]
        public int ConflictId { get; set; }

        // Foreign Key - first event involved in conflict
        [Required]
        public int FirstEventId { get; set; }

        // Foreign Key - second event involved in conflict
        [Required]
        public int SecondEventId { get; set; }

       
        [Required]
        public DateTime ConflictStartTime { get; set; }

       
        [Required]
        public DateTime ConflictEndTime { get; set; }

        
        [Required]
        public TimeSpan OverlapDuration { get; set; }

        // Severity level: "Minor", "Moderate", "Severe"
       
        [Required]
        [StringLength(50)]
        public string ConflictSeverity { get; set; }

        
        [Required]
        public DateTime DetectedAt { get; set; }

        
        public bool IsResolved { get; set; }

        // Navigation properties - link to the two conflicting events
        [ForeignKey("FirstEventId")]
        public virtual Event FirstEvent { get; set; }

        [ForeignKey("SecondEventId")]
        public virtual Event SecondEvent { get; set; }

        // Constructor 
        public Conflict()
        {
            DetectedAt = DateTime.Now;
            IsResolved = false;
            ConflictSeverity = "Minor";
        }

       
        // Gets the overlap duration in minutes
       
        public int GetOverlapMinutes()
        {
            return (int)OverlapDuration.TotalMinutes;
        }

     
        // Generates a human-readable description of the conflict
       
        public string GetConflictDescription()
        {
            if (FirstEvent == null || SecondEvent == null)
                return "Conflict detected between two events.";

            return $"'{FirstEvent.Title}' conflicts with '{SecondEvent.Title}' " +
                   $"on {ConflictStartTime.ToString("MMM dd, yyyy")} " +
                   $"from {ConflictStartTime.ToString("h:mm tt")} to {ConflictEndTime.ToString("h:mm tt")} " +
                   $"({GetOverlapMinutes()} minutes overlap).";
        }

        
        // Marks this conflict as resolved/acknowledged by the user
      
        public void MarkAsResolved()
        {
            IsResolved = true;
        }

       
        // Calculates the severity of the conflict based on overlap duration
       
        public string CalculateSeverity()
        {
            int overlapMinutes = GetOverlapMinutes();

            if (overlapMinutes >= 60) // 1 hour or more
                return "Severe";
            else if (overlapMinutes >= 30) // 30-60 minutes
                return "Moderate";
            else
                return "Minor"; // Less than 30 minutes
        }

       
        // Generates suggestions for resolving the conflict
        
        public string[] GetResolutionSuggestions()
        {
            if (FirstEvent == null || SecondEvent == null)
                return new string[] { "Load event details to see suggestions." };

            var suggestions = new string[]
            {
                $"Move '{FirstEvent.Title}' to {ConflictEndTime.AddMinutes(15).ToString("h:mm tt")}",
                $"Move '{SecondEvent.Title}' to {ConflictEndTime.AddMinutes(15).ToString("h:mm tt")}",
                $"Shorten '{FirstEvent.Title}' to end before {ConflictStartTime.ToString("h:mm tt")}",
                $"Shorten '{SecondEvent.Title}' to end before {ConflictStartTime.ToString("h:mm tt")}"
            };

            return suggestions;
        }

       
        // Returns a formatted string representation of the conflict
        
        public override string ToString()
        {
            return $"[{ConflictSeverity}] Conflict on {ConflictStartTime.ToString("MMM dd, yyyy")} - " +
                   $"{GetOverlapMinutes()} min overlap";
        }

        
        // Static method to create a conflict from two events
        
        public static Conflict CreateFromEvents(Event event1, Event event2)
        {
            // Check if events actually overlap
            if (!event1.HasTimeConflict(event2))
                return null;

            // Calculate overlap times
            DateTime conflictStart = event1.StartTime > event2.StartTime ? event1.StartTime : event2.StartTime;
            DateTime conflictEnd = event1.EndTime < event2.EndTime ? event1.EndTime : event2.EndTime;

            var conflict = new Conflict
            {
                FirstEventId = event1.EventId,
                SecondEventId = event2.EventId,
                ConflictStartTime = conflictStart,
                ConflictEndTime = conflictEnd,
                OverlapDuration = conflictEnd - conflictStart,
                FirstEvent = event1,
                SecondEvent = event2
            };

            // Calculate severity
            conflict.ConflictSeverity = conflict.CalculateSeverity();

            return conflict;
        }
    }
}

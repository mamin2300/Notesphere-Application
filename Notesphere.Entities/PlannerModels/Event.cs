using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Author: Malika Muskan (991808527)
// Description: Event entity class representing a single calendar event in the timetable.
// This class maps to the Events table in the database.


namespace Notesphere.Entities.PlannerModels
{

    public class Event
    {
        // Primary Key
        [Key]
        public int EventId { get; set; }

        // Foreign Key - links to User who owns this event
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        // Event Details
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        // Color for visual display (hex code like #FF5733)
        [Required]
        [StringLength(7)] // Format: #RRGGBB
        public string ColorCode { get; set; }

        // Event type: Class, Assignment, Exam, Lab, Meeting, etc.
        [Required]
        [StringLength(50)]
        public string EventType { get; set; }

        // Is this an all-day event?
        public bool IsAllDay { get; set; }

        // Foreign Key - links to RecurringEvent if this is part of a recurring series
        // Nullable because not all events are recurring
        public int? RecurrenceId { get; set; }

        // Additional notes for the event
        [StringLength(2000)]
        public string Notes { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation property - link to RecurringEvent
         [ForeignKey("RecurrenceId")]
         public virtual RecurringEvent RecurringEvent { get; set; }


        // Constructor - sets default values
        public Event()
        {
            CreatedAt = DateTime.Now;
            IsAllDay = false;
            ColorCode = "#49111C"; // burgundy color for color scheme
        }



        // Calculates the duration of the event
        public TimeSpan GetDuration()
        {
            return EndTime - StartTime;
        }

        // Checks if this event occurs on a specific date

        public bool IsOnDate(DateTime date)
        {
            return StartTime.Date == date.Date ||
                   (StartTime.Date <= date.Date && EndTime.Date >= date.Date);
        }


        // Checks if this event has a time conflict with another event

        public bool HasTimeConflict(Event other)
        {
            // Events conflict if they overlap in time
            return this.StartTime < other.EndTime && this.EndTime > other.StartTime;
        }


        // Creates a copy of this event (useful for recurring events)
        public Event Clone()
        {
            return new Event
            {
                UserId = this.UserId,
                Title = this.Title,
                Description = this.Description,
                Location = this.Location,
                ColorCode = this.ColorCode,
                EventType = this.EventType,
                IsAllDay = this.IsAllDay,
                Notes = this.Notes,
                RecurrenceId = this.RecurrenceId
            };
        }


        // Validates if the event data is correct
        public bool Validate()
        {
            // End time must be after start time
            if (EndTime <= StartTime)
                return false;


            if (string.IsNullOrWhiteSpace(Title))
                return false;

            // Color code must be valid hex format
            if (!ColorCode.StartsWith("#") || ColorCode.Length != 7)
                return false;

            return true;
        }
    }
}

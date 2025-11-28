using Notesphere.Entities.PlannerModels;
using Notesphere.Services.PlannerRepository;
namespace Notesphere.Operations.PlannerServices
{
    
    // Main service class for Planner module business logic.
    // Handles event management, conflict detection, and recurring events.
    
    public class PlannerService
    {
        private readonly IPlannerService _repository;
        private readonly ConflictDetector _conflictDetector;
        private readonly RecurrenceEngine _recurrenceEngine;

      
        // Constructor - injects dependencies
        public PlannerService(
            IPlannerService repository,
            ConflictDetector conflictDetector,
            RecurrenceEngine recurrenceEngine)
        {
            _repository = repository;
            _conflictDetector = conflictDetector;
            _recurrenceEngine = recurrenceEngine;
        }

        #region Event Management

        
        // Creates a new event and checks for conflicts
        public async Task<int> CreateEventAsync(Event newEvent, string userId)
        {
            try
            {
                // Validate event
                if (!newEvent.Validate())
                    throw new ArgumentException("Invalid event data");

                // Set user ID
                newEvent.UserId = userId;

                // Check for conflicts with existing events
                var existingEvents = await _repository.GetEventsByDateRangeAsync(
                    userId,
                    newEvent.StartTime.Date,
                    newEvent.EndTime.Date.AddDays(1));

                var conflicts = _conflictDetector.CheckNewEventConflicts(newEvent, existingEvents);

                // Add the event
                int eventId = await _repository.AddEventAsync(newEvent);

                // If conflicts detected, save them to database
                if (conflicts.Any())
                {
                    foreach (var conflict in conflicts)
                    {
                        conflict.FirstEventId = eventId;
                        await _repository.AddConflictAsync(conflict);
                    }
                }

                return eventId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating event: {ex.Message}");
            }
        }

        // Updates an existing event
        
        public async Task<bool> UpdateEventAsync(int eventId, Event updatedEvent, string userId)
        {
            try
            {
                // Get existing event
                var existingEvent = await _repository.GetEventByIdAsync(eventId, userId);
                if (existingEvent == null)
                    return false;

                // Validate updated data
                if (!updatedEvent.Validate())
                    throw new ArgumentException("Invalid event data");

                // Update the event
                updatedEvent.EventId = eventId;
                updatedEvent.UserId = userId;
                bool success = await _repository.UpdateEventAsync(updatedEvent);

                if (success)
                {
                    // Re-check conflicts after update
                    await RedetectConflictsForEventAsync(eventId, userId);
                }

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating event: {ex.Message}");
            }
        }

      
        // Deletes an event
        public async Task<bool> DeleteEventAsync(int eventId, string userId)
        {
            try
            {
                // Delete any conflicts involving this event
                var allConflicts = await _repository.GetAllConflictsAsync(userId);
                var relatedConflicts = allConflicts
                    .Where(c => c.FirstEventId == eventId || c.SecondEventId == eventId)
                    .ToList();

                foreach (var conflict in relatedConflicts)
                {
                    await _repository.DeleteConflictAsync(conflict.ConflictId);
                }

                // Delete the event
                return await _repository.DeleteEventAsync(eventId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting event: {ex.Message}");
            }
        }

        
        // Gets a single event by ID
        public async Task<Event> GetEventByIdAsync(int eventId, string userId)
        {
            return await _repository.GetEventByIdAsync(eventId, userId);
        }

      
        // Gets all events for a user
        public async Task<List<Event>> GetAllEventsAsync(string userId)
        {
            return await _repository.GetAllEventsAsync(userId);
        }

       
        // Gets events by type
        public async Task<List<Event>> GetEventsByTypeAsync(string userId, string eventType)
        {
            return await _repository.GetEventsByTypeAsync(userId, eventType);
        }

        #endregion

        #region Recurring Events

       
        // Creates a recurring event pattern and generates initial occurrences
        
        public async Task<int> CreateRecurringEventAsync(Event baseEvent, RecurringEvent recurrence, string userId)
        {
            try
            {
                // Validate recurrence pattern
                if (!recurrence.ValidateRecurrence())
                    throw new ArgumentException("Invalid recurrence pattern");

                // Save the recurrence pattern first
                int recurrenceId = await _repository.SaveRecurringEventAsync(recurrence);

                // Set the recurrence ID on the base event
                baseEvent.RecurrenceId = recurrenceId;
                baseEvent.UserId = userId;

                // Save the base/parent event
                int parentEventId = await _repository.AddEventAsync(baseEvent);

                // Update recurrence with parent event ID
                recurrence.ParentEventId = parentEventId;
                await _repository.UpdateRecurringEventAsync(recurrence);

                // Generate occurrences for the next 6 months
                DateTime endDate = DateTime.Now.AddMonths(6);
                var occurrences = _recurrenceEngine.GenerateOccurrences(
                    baseEvent,
                    recurrence,
                    recurrence.RecurrenceStartDate,
                    endDate);

                // Save all generated occurrences
                foreach (var occurrence in occurrences)
                {
                    occurrence.UserId = userId;
                    occurrence.RecurrenceId = recurrenceId;
                    await _repository.AddEventAsync(occurrence);
                }

                return recurrenceId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating recurring event: {ex.Message}");
            }
        }

        
        // Deletes a recurring event and optionally all its occurrences
        
        public async Task<bool> DeleteRecurringEventAsync(int recurrenceId, bool deleteAllOccurrences, string userId)
        {
            return await _repository.DeleteRecurringEventAsync(recurrenceId, deleteAllOccurrences);
        }

        #endregion

        #region Calendar Views

        
        // Gets events for weekly planner view
       
        public async Task<List<Event>> GetWeeklyPlannerAsync(string userId, DateTime weekStartDate)
        {
            try
            {
                // Calculate week end (7 days from start)
                DateTime weekEndDate = weekStartDate.AddDays(7);

                // Get all events in this week
                var events = await _repository.GetEventsByDateRangeAsync(userId, weekStartDate, weekEndDate);

                // Expand any recurring events for this week
                var expandedEvents = _recurrenceEngine.ExpandRecurringEvents(events, weekStartDate, weekEndDate);

                // Sort by start time
                return expandedEvents.OrderBy(e => e.StartTime).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting weekly planner: {ex.Message}");
            }
        }

       
        // Gets events for a specific date range
       
        public async Task<List<Event>> GetEventsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var events = await _repository.GetEventsByDateRangeAsync(userId, startDate, endDate);

                // Expand recurring events
                var expandedEvents = _recurrenceEngine.ExpandRecurringEvents(events, startDate, endDate);

                return expandedEvents.OrderBy(e => e.StartTime).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting events by date range: {ex.Message}");
            }
        }

        #endregion

        #region Conflict Management

        
        // Detects all conflicts for a user's schedule
        
        public async Task<List<Conflict>> DetectAllConflictsAsync(string userId)
        {
            try
            {
                // Get all user's events
                var allEvents = await _repository.GetAllEventsAsync(userId);

                // Detect conflicts
                var conflicts = _conflictDetector.DetectConflicts(allEvents);

                // Save new conflicts to database
                var existingConflicts = await _repository.GetAllConflictsAsync(userId);

                foreach (var conflict in conflicts)
                {
                    // Check if this conflict already exists
                    bool alreadyExists = existingConflicts.Any(ec =>
                        (ec.FirstEventId == conflict.FirstEventId && ec.SecondEventId == conflict.SecondEventId) ||
                        (ec.FirstEventId == conflict.SecondEventId && ec.SecondEventId == conflict.FirstEventId));

                    if (!alreadyExists)
                    {
                        await _repository.AddConflictAsync(conflict);
                    }
                }

                return conflicts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error detecting conflicts: {ex.Message}");
            }
        }

       
        // Gets all unresolved conflicts for a user
        public async Task<List<Conflict>> GetUnresolvedConflictsAsync(string userId)
        {
            return await _repository.GetUnresolvedConflictsAsync(userId);
        }

       
        // Marks a conflict as resolved
        public async Task<bool> ResolveConflictAsync(int conflictId)
        {
            return await _repository.ResolveConflictAsync(conflictId);
        }

     
        // Re-detects conflicts after an event is updated
        private async Task RedetectConflictsForEventAsync(int eventId, string userId)
        {
            // Delete old conflicts for this event
            var allConflicts = await _repository.GetAllConflictsAsync(userId);
            var oldConflicts = allConflicts
                .Where(c => c.FirstEventId == eventId || c.SecondEventId == eventId)
                .ToList();

            foreach (var oldConflict in oldConflicts)
            {
                await _repository.DeleteConflictAsync(oldConflict.ConflictId);
            }

            // Get the updated event
            var updatedEvent = await _repository.GetEventByIdAsync(eventId, userId);

            // Get all other events
            var allEvents = await _repository.GetAllEventsAsync(userId);
            var otherEvents = allEvents.Where(e => e.EventId != eventId).ToList();

            // Check for new conflicts
            var newConflicts = _conflictDetector.CheckNewEventConflicts(updatedEvent, otherEvents);

            // Save new conflicts
            foreach (var conflict in newConflicts)
            {
                await _repository.AddConflictAsync(conflict);
            }
        }

        #endregion

        #region Helper Methods

        
        // Gets upcoming events for a user (next 7 days)
        public async Task<List<Event>> GetUpcomingEventsAsync(string userId, int days = 7)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(days);

            return await GetEventsByDateRangeAsync(userId, startDate, endDate);
        }

        
        // Gets events for today
        public async Task<List<Event>> GetTodayEventsAsync(string userId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            return await GetEventsByDateRangeAsync(userId, today, tomorrow);
        }

        #endregion
    }

}

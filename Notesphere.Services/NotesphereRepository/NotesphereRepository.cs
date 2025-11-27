using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.NotesModels;
using Notesphere.Entities.PlannerModels;
using Notesphere.Services.NotesphereDataAccessLayer;
using Notesphere.Services.NotesphereRepository;

namespace Notesphere.Services.NotesRepository
{
    public class NotesphereRepository : INotesphereService
    {
        private readonly NotesphereDbContext _db;

        public NotesphereRepository(NotesphereDbContext db)
        {
            _db = db;
        }
        // Notes CRUD operations

        public async Task<List<Note>> GetAllNotes() =>
            await _db.Notes.ToListAsync();

        public async Task<Note?> GetNoteById(int id) =>
            await _db.Notes.FirstOrDefaultAsync(n => n.Id == id);

        public async Task AddNote(Note note)
        {
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateNote(Note note)
        {
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteNote(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note != null)
            {
                _db.Notes.Remove(note);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<NoteTemplate>> GetTemplates() =>
            await _db.NoteTemplates.ToListAsync();

        public async Task<List<Tag>> GetAllTags() =>
            await _db.Tags.ToListAsync();

        public async Task SaveNoteVersion(NoteVersion version)
        {
            _db.NoteVersions.Add(version);
            await _db.SaveChangesAsync();
        }

        public async Task AddTagToNote(NoteTag join)
        {
            _db.NoteTags.Add(join);
            await _db.SaveChangesAsync();
        }

        //planner CRUD operations
        
        // Gets a single event by its ID for a specific user
        public async Task<Event> GetEventByIdAsync(int eventId, string userId)
        {
            try
            {
                return await _db.Events
                    .Include(e => e.RecurringEvent)
                    .FirstOrDefaultAsync(e => e.EventId == eventId && e.UserId == userId);
            }
            catch (Exception ex)
            {
                // Log error here
                throw new Exception($"Error retrieving event with ID {eventId}: {ex.Message}");
            }
        }

       
        // Gets all events for a specific user
        public async Task<List<Event>> GetAllEventsAsync(string userId)
        {
            try
            {
                return await _db.Events
                    .Include(e => e.RecurringEvent)
                    .Where(e => e.UserId == userId)
                    .OrderBy(e => e.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events for user {userId}: {ex.Message}");
            }
        }

        
        // Gets events within a specific date range for a user
        public async Task<List<Event>> GetEventsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _db.Events
                    .Include(e => e.RecurringEvent)
                    .Where(e => e.UserId == userId
                             && e.StartTime >= startDate
                             && e.EndTime <= endDate)
                    .OrderBy(e => e.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events by date range: {ex.Message}");
            }
        }

        
        // Gets events by type (Class, Assignment, Exam, etc.)
        public async Task<List<Event>> GetEventsByTypeAsync(string userId, string eventType)
        {
            try
            {
                return await _db.Events
                    .Include(e => e.RecurringEvent)
                    .Where(e => e.UserId == userId && e.EventType == eventType)
                    .OrderBy(e => e.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events by type {eventType}: {ex.Message}");
            }
        }

        
        // Adds a new event to the database
        public async Task<int> AddEventAsync(Event eventToAdd)
        {
            try
            {
                // Validate event before adding
                if (!eventToAdd.Validate())
                    throw new ArgumentException("Invalid event data");

                eventToAdd.CreatedAt = DateTime.Now;

                await _db.Events.AddAsync(eventToAdd);
                await _db.SaveChangesAsync();

                return eventToAdd.EventId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding event: {ex.Message}");
            }
        }

        
        // Updates an existing event
        public async Task<bool> UpdateEventAsync(Event eventToUpdate)
        {
            try
            {
                // Check if event exists
                var existingEvent = await _db.Events
                    .FirstOrDefaultAsync(e => e.EventId == eventToUpdate.EventId);

                if (existingEvent == null)
                    return false;

                // Validate updated data
                if (!eventToUpdate.Validate())
                    throw new ArgumentException("Invalid event data");

                // Update properties
                existingEvent.Title = eventToUpdate.Title;
                existingEvent.Description = eventToUpdate.Description;
                existingEvent.StartTime = eventToUpdate.StartTime;
                existingEvent.EndTime = eventToUpdate.EndTime;
                existingEvent.Location = eventToUpdate.Location;
                existingEvent.ColorCode = eventToUpdate.ColorCode;
                existingEvent.EventType = eventToUpdate.EventType;
                existingEvent.IsAllDay = eventToUpdate.IsAllDay;
                existingEvent.Notes = eventToUpdate.Notes;
                existingEvent.ModifiedAt = DateTime.Now;

                _db.Events.Update(existingEvent);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating event: {ex.Message}");
            }
        }

       
        /// Deletes an event by ID
        public async Task<bool> DeleteEventAsync(int eventId, string userId)
        {
            try
            {
                var eventToDelete = await _db.Events
                    .FirstOrDefaultAsync(e => e.EventId == eventId && e.UserId == userId);

                if (eventToDelete == null)
                    return false;

                _db.Events.Remove(eventToDelete);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting event: {ex.Message}");
            }
        }

        // Recurring Event Operations

        public async Task<RecurringEvent> GetRecurringEventByIdAsync(int recurrenceId)
        {
            try
            {
                return await _db.RecurringEvents
                    .Include(r => r.Events)
                    .FirstOrDefaultAsync(r => r.RecurrenceId == recurrenceId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recurring event: {ex.Message}");
            }
        }

       
        // Gets all recurring events for a user
        public async Task<List<RecurringEvent>> GetAllRecurringEventsAsync(string userId)
        {
            try
            {
                return await _db.RecurringEvents
                    .Include(r => r.Events)
                    .Where(r => r.Events.Any(e => e.UserId == userId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recurring events: {ex.Message}");
            }
        }

       
        // Saves a new recurring event pattern
        public async Task<int> SaveRecurringEventAsync(RecurringEvent recurringEvent)
        {
            try
            {
                // Validate recurrence pattern
                if (!recurringEvent.ValidateRecurrence())
                    throw new ArgumentException("Invalid recurrence pattern");

                await _db.RecurringEvents.AddAsync(recurringEvent);
                await _db.SaveChangesAsync();

                return recurringEvent.RecurrenceId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving recurring event: {ex.Message}");
            }
        }

        
        // Updates an existing recurring event pattern
        public async Task<bool> UpdateRecurringEventAsync(RecurringEvent recurringEvent)
        {
            try
            {
                var existing = await _db.RecurringEvents
                    .FirstOrDefaultAsync(r => r.RecurrenceId == recurringEvent.RecurrenceId);

                if (existing == null)
                    return false;

                // Validate updated pattern
                if (!recurringEvent.ValidateRecurrence())
                    throw new ArgumentException("Invalid recurrence pattern");

                // Update properties
                existing.Pattern = recurringEvent.Pattern;
                existing.Interval = recurringEvent.Interval;
                existing.DaysOfWeek = recurringEvent.DaysOfWeek;
                existing.RecurrenceStartDate = recurringEvent.RecurrenceStartDate;
                existing.RecurrenceEndDate = recurringEvent.RecurrenceEndDate;
                existing.MaxOccurrences = recurringEvent.MaxOccurrences;
                existing.ExceptionDates = recurringEvent.ExceptionDates;
                existing.IsActive = recurringEvent.IsActive;

                _db.RecurringEvents.Update(existing);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating recurring event: {ex.Message}");
            }
        }

        
        // Deletes a recurring event pattern and optionally all its occurrences
        public async Task<bool> DeleteRecurringEventAsync(int recurrenceId, bool deleteAllOccurrences)
        {
            try
            {
                var recurringEvent = await _db.RecurringEvents
                    .Include(r => r.Events)
                    .FirstOrDefaultAsync(r => r.RecurrenceId == recurrenceId);

                if (recurringEvent == null)
                    return false;

                // If deleting all occurrences, remove all related events
                if (deleteAllOccurrences && recurringEvent.Events != null)
                {
                    _db.Events.RemoveRange(recurringEvent.Events);
                }
                else
                {
                    // Just unlink events from this recurrence pattern
                    foreach (var evt in recurringEvent.Events)
                    {
                        evt.RecurrenceId = null;
                    }
                }

                _db.RecurringEvents.Remove(recurringEvent);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting recurring event: {ex.Message}");
            }
        }

        // Conflict Operations

        public async Task<List<Conflict>> GetAllConflictsAsync(string userId)
        {
            try
            {
                return await _db.Conflicts
                    .Include(c => c.FirstEvent)
                    .Include(c => c.SecondEvent)
                    .Where(c => c.FirstEvent.UserId == userId || c.SecondEvent.UserId == userId)
                    .OrderByDescending(c => c.DetectedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving conflicts: {ex.Message}");
            }
        }

        
        // Gets unresolved conflicts for a user
        public async Task<List<Conflict>> GetUnresolvedConflictsAsync(string userId)
        {
            try
            {
                return await _db.Conflicts
                    .Include(c => c.FirstEvent)
                    .Include(c => c.SecondEvent)
                    .Where(c => !c.IsResolved
                             && (c.FirstEvent.UserId == userId || c.SecondEvent.UserId == userId))
                    .OrderByDescending(c => c.DetectedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving unresolved conflicts: {ex.Message}");
            }
        }

        // Gets conflicts within a specific date range
        public async Task<List<Conflict>> GetConflictsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _db.Conflicts
                    .Include(c => c.FirstEvent)
                    .Include(c => c.SecondEvent)
                    .Where(c => (c.FirstEvent.UserId == userId || c.SecondEvent.UserId == userId)
                             && c.ConflictStartTime >= startDate
                             && c.ConflictEndTime <= endDate)
                    .OrderBy(c => c.ConflictStartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving conflicts by date range: {ex.Message}");
            }
        }

       
        // Adds a detected conflict to the database
        public async Task<int> AddConflictAsync(Conflict conflict)
        {
            try
            {
                conflict.DetectedAt = DateTime.Now;

                await _db.Conflicts.AddAsync(conflict);
                await _db.SaveChangesAsync();

                return conflict.ConflictId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding conflict: {ex.Message}");
            }
        }

       
        // Marks a conflict as resolved
        public async Task<bool> ResolveConflictAsync(int conflictId)
        {
            try
            {
                var conflict = await _db.Conflicts
                    .FirstOrDefaultAsync(c => c.ConflictId == conflictId);

                if (conflict == null)
                    return false;

                conflict.MarkAsResolved();

                _db.Conflicts.Update(conflict);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resolving conflict: {ex.Message}");
            }
        }

       
        // Deletes a conflict record
        public async Task<bool> DeleteConflictAsync(int conflictId)
        {
            try
            {
                var conflict = await _db.Conflicts
                    .FirstOrDefaultAsync(c => c.ConflictId == conflictId);

                if (conflict == null)
                    return false;

                _db.Conflicts.Remove(conflict);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting conflict: {ex.Message}");
            }
        }
        //sharing CRUD operations
        //productivity CRUD operations
    }
}

using Notesphere.Entities.NotesModels;
using Notesphere.Entities.PlannerModels;

namespace Notesphere.Services.NotesRepository
{
    public interface INotesphereService
    {
        // Notes CRUD operations
        Task<List<Note>> GetAllNotes();
        Task<Note?> GetNoteById(int id);
        Task AddNote(Note note);
        Task UpdateNote(Note note);
        Task DeleteNote(int id);

        Task<List<NoteTemplate>> GetTemplates();
        Task<List<Tag>> GetAllTags();
        Task SaveNoteVersion(NoteVersion Version);
        Task AddTagToNote(NoteTag join);

        //Planner CRUD operations

        // Gets a single event by its ID for a specific user
        Task<Event> GetEventByIdAsync(int eventId, string userId);

        
        // Gets all events for a specific user
        Task<List<Event>> GetAllEventsAsync(string userId);

       
        // Gets events within a specific date range for a user
        Task<List<Event>> GetEventsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

      
        // Gets events by type (Class, Assignment, Exam, etc.)
        Task<List<Event>> GetEventsByTypeAsync(string userId, string eventType);

        
        // Adds a new event to the database
        Task<int> AddEventAsync(Event eventToAdd);

       
        // Updates an existing event
        Task<bool> UpdateEventAsync(Event eventToUpdate);

        
        // Deletes an event by ID
        Task<bool> DeleteEventAsync(int eventId, string userId);

        
        // Gets a recurring event pattern by its ID
        Task<RecurringEvent> GetRecurringEventByIdAsync(int recurrenceId);

        // Gets all recurring events for a user
        Task<List<RecurringEvent>> GetAllRecurringEventsAsync(string userId);

        
        // Saves a new recurring event pattern
        Task<int> SaveRecurringEventAsync(RecurringEvent recurringEvent);

        
        // Updates an existing recurring event pattern
        Task<bool> UpdateRecurringEventAsync(RecurringEvent recurringEvent);

        
        // Deletes a recurring event pattern and optionally all its occurrences
        Task<bool> DeleteRecurringEventAsync(int recurrenceId, bool deleteAllOccurrences);

        
        // Gets all conflicts for a user
        Task<List<Conflict>> GetAllConflictsAsync(string userId);

        
        // Gets unresolved conflicts for a user
        Task<List<Conflict>> GetUnresolvedConflictsAsync(string userId);

        
        // Gets conflicts within a specific date range
        Task<List<Conflict>> GetConflictsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);

        
        /// Adds a detected conflict to the database
        Task<int> AddConflictAsync(Conflict conflict);

      
        // Marks a conflict as resolved
        Task<bool> ResolveConflictAsync(int conflictId);

      
        // Deletes a conflict record
        Task<bool> DeleteConflictAsync(int conflictId);

        //Sharing CRUD operations
        //Productivity CRUD operations
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Notesphere.Entities.SharingModels;

namespace Notesphere.Services.SharingRepository
{
    public interface ISharingServices
    {
        // Group spaces
        Task<List<GroupSpace>> GetGroupSpacesForUserAsync(int studentUserId);
        Task<GroupSpace?> GetGroupSpaceByIdAsync(int id);
        Task<GroupSpace> CreateGroupSpaceAsync(GroupSpace groupSpace);
        Task<bool> AddMemberAsync(int groupSpaceId, int studentUserId, GroupRole role);
        Task<bool> RemoveMemberAsync(int groupSpaceId, int studentUserId);

        // Sharing notes
        Task<SharedNote> ShareNoteWithUserAsync(int noteId, int fromUserId, int toUserId, bool canEdit);
        Task<SharedNote> ShareNoteWithGroupAsync(int noteId, int fromUserId, int groupSpaceId, bool canEdit);
        Task<List<SharedNote>> GetNotesSharedWithUserAsync(int studentUserId);
        Task<bool> RevokeShareAsync(int sharedNoteId);

        // Discussion
        Task<List<NoteComment>> GetCommentsForNoteAsync(int noteId);
        Task<NoteComment> AddCommentAsync(int noteId, int authorUserId, string content, int? parentCommentId = null);
        Task<bool> DeleteCommentAsync(int commentId, int requestingUserId, bool isAdmin);
    }
}

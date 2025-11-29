using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.SharingModels;
using Notesphere.Services.NotesphereDataAccessLayer;

namespace Notesphere.Services.SharingRepository
{
    public class SharingRepository : ISharingServices
    {
        private readonly NotesphereDbContext _dbContext;

        public SharingRepository(NotesphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<GroupSpace>> GetGroupSpacesForUserAsync(int studentUserId)
        {
            return await _dbContext.GroupMembers
                .Where(m => m.StudentUserId == studentUserId)
                .Select(m => m.GroupSpace!)
                .Where(g => g.IsActive)
                .ToListAsync();
        }

        public async Task<GroupSpace?> GetGroupSpaceByIdAsync(int id)
        {
            return await _dbContext.GroupSpaces
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<GroupSpace> CreateGroupSpaceAsync(GroupSpace groupSpace)
        {
            groupSpace.CreatedAt = DateTime.UtcNow;
            groupSpace.IsActive = true;

            groupSpace.Members.Add(new GroupMember
            {
                StudentUserId = groupSpace.OwnerId,
                Role = GroupRole.Owner,
                JoinedAt = DateTime.UtcNow
            });

            _dbContext.GroupSpaces.Add(groupSpace);
            await _dbContext.SaveChangesAsync();

            return groupSpace;
        }

        public async Task<bool> AddMemberAsync(int groupSpaceId, int studentUserId, GroupRole role)
        {
            bool exists = await _dbContext.GroupMembers
                .AnyAsync(m => m.GroupSpaceId == groupSpaceId && m.StudentUserId == studentUserId);

            if (exists) return false;

            _dbContext.GroupMembers.Add(new GroupMember
            {
                GroupSpaceId = groupSpaceId,
                StudentUserId = studentUserId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberAsync(int groupSpaceId, int studentUserId)
        {
            var member = await _dbContext.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupSpaceId == groupSpaceId && m.StudentUserId == studentUserId);

            if (member == null) return false;

            _dbContext.GroupMembers.Remove(member);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<SharedNote> ShareNoteWithUserAsync(int noteId, int fromUserId, int toUserId, bool canEdit)
        {
            var share = new SharedNote
            {
                NoteId = noteId,
                SharedByUserId = fromUserId,
                SharedWithUserId = toUserId,
                TargetType = ShareTargetType.Individual,
                CanEdit = canEdit,
                SharedAt = DateTime.UtcNow
            };

            _dbContext.SharedNotes.Add(share);
            await _dbContext.SaveChangesAsync();
            return share;
        }

        public async Task<SharedNote> ShareNoteWithGroupAsync(int noteId, int fromUserId, int groupSpaceId, bool canEdit)
        {
            var share = new SharedNote
            {
                NoteId = noteId,
                SharedByUserId = fromUserId,
                GroupSpaceId = groupSpaceId,
                TargetType = ShareTargetType.Group,
                CanEdit = canEdit,
                SharedAt = DateTime.UtcNow
            };

            _dbContext.SharedNotes.Add(share);
            await _dbContext.SaveChangesAsync();
            return share;
        }

        public async Task<List<SharedNote>> GetNotesSharedWithUserAsync(int studentUserId)
        {
            var groupIds = await _dbContext.GroupMembers
                .Where(m => m.StudentUserId == studentUserId)
                .Select(m => m.GroupSpaceId)
                .ToListAsync();

            return await _dbContext.SharedNotes
                .Where(sn =>
                    sn.SharedWithUserId == studentUserId ||
                    (sn.GroupSpaceId != null && groupIds.Contains(sn.GroupSpaceId.Value)))
                .ToListAsync();
        }

        public async Task<bool> RevokeShareAsync(int sharedNoteId)
        {
            var share = await _dbContext.SharedNotes.FindAsync(sharedNoteId);
            if (share == null) return false;

            _dbContext.SharedNotes.Remove(share);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<List<NoteComment>> GetCommentsForNoteAsync(int noteId)
        {
            return await _dbContext.NoteComments
                .Where(c => c.NoteId == noteId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<NoteComment> AddCommentAsync(int noteId, int authorUserId, string content, int? parentCommentId = null)
        {
            var comment = new NoteComment
            {
                NoteId = noteId,
                AuthorUserId = authorUserId,
                Content = content,
                ParentCommentId = parentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.NoteComments.Add(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int requestingUserId, bool isAdmin)
        {
            var comment = await _dbContext.NoteComments.FindAsync(commentId);
            if (comment == null) return false;

            if (!isAdmin && comment.AuthorUserId != requestingUserId)
            {
                return false;
            }

            _dbContext.NoteComments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

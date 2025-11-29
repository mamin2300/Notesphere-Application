using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.SharingModels
{
    public enum GroupRole
    {
        Owner = 0,
        Admin = 1,
        Member = 2
    }

    public enum ShareTargetType
    {
        Individual = 0,
        Group = 1
    }

    /// A collaborative group space where students can share notes.

    public class GroupSpace
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int OwnerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<SharedNote> SharedNotes { get; set; } = new List<SharedNote>();
    }

    /// Membership of a student in a group, with a role.
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }

        public int GroupSpaceId { get; set; }
        public int StudentUserId { get; set; }

        public GroupRole Role { get; set; } = GroupRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public GroupSpace? GroupSpace { get; set; }
    }

    /// A record that a note is shared with an individual user or a group.
    public class SharedNote
    {
        [Key]
        public int Id { get; set; }

        public int NoteId { get; set; }     
        public int SharedByUserId { get; set; }  

        public int? SharedWithUserId { get; set; }
        public int? GroupSpaceId { get; set; }

        public ShareTargetType TargetType { get; set; }

        public bool CanEdit { get; set; } = false;
        public DateTime SharedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public GroupSpace? GroupSpace { get; set; }

        public ICollection<NoteComment> Comments { get; set; } = new List<NoteComment>();
    }

    public class NoteComment
    {
        [Key]
        public int Id { get; set; }

        public int NoteId { get; set; }
        public int AuthorUserId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ParentCommentId { get; set; }

        public NoteComment? ParentComment { get; set; }
        public ICollection<NoteComment> Replies { get; set; } = new List<NoteComment>();
    }
}

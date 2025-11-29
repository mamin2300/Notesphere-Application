using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notesphere.Operations.Models.Sharing
{
    // List item when showing user's groups
    public class GroupSpaceListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public string Role { get; set; } = string.Empty;   
    }

    // Form for creating a group
    public class CreateGroupSpaceViewModel
    {
        [Required]
        [Display(Name = "Group Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public enum ShareTargetTypeViewModel
    {
        Individual = 0,
        Group = 1
    }

    // Form for sharing a note
    public class ShareNoteViewModel
    {
        [Required]
        public int NoteId { get; set; }

        [Required]
        [Display(Name = "Share With")]
        public ShareTargetTypeViewModel TargetType { get; set; }

        [Display(Name = "User Id (for individual share)")]
        public int? TargetUserId { get; set; }

        [Display(Name = "Group Id (for group share)")]
        public int? TargetGroupId { get; set; }

        [Display(Name = "Allow recipient to edit")]
        public bool CanEdit { get; set; }
    }

    // Row when listing notes shared with current user
    public class SharedNoteListItemViewModel
    {
        public int SharedNoteId { get; set; }
        public int NoteId { get; set; }
        public string SharedBy { get; set; } = string.Empty;
        public string? SharedTo { get; set; }
        public bool CanEdit { get; set; }
        public string TargetType { get; set; } = string.Empty;
    }

    // For adding comments
    public class NoteCommentInputViewModel
    {
        [Required]
        public int NoteId { get; set; }

        public int? ParentCommentId { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Content { get; set; } = string.Empty;
    }

    // For displaying comments tree
    public class NoteCommentDisplayViewModel
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string AuthorName { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;

        public List<NoteCommentDisplayViewModel> Replies { get; set; }
            = new List<NoteCommentDisplayViewModel>();
    }
}

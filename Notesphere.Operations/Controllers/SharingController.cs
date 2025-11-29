using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notesphere.Entities.SharingModels;
using Notesphere.Operations.Models.Sharing;
using Notesphere.Services.SharingRepository;

namespace Notesphere.Operations.Controllers
{
    public class SharingController : Controller
    {
        private readonly ISharingServices _sharingServices;

        public SharingController(ISharingServices sharingServices)
        {
            _sharingServices = sharingServices;
        }

        // TODO: replace this with real logged-in student id when Identity is ready
        private int GetCurrentStudentUserId()
        {
            
            return 1;
        }

        public async Task<IActionResult> MyGroups()
        {
            var userId = GetCurrentStudentUserId();
            var groups = await _sharingServices.GetGroupSpacesForUserAsync(userId);

            var model = groups.Select(g => new GroupSpaceListItemViewModel
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                MemberCount = g.Members?.Count ?? 0,
                Role = g.Members
                    .FirstOrDefault(m => m.StudentUserId == userId)?.Role.ToString() ?? "Member"
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateGroup()
        {
            return View(new CreateGroupSpaceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(CreateGroupSpaceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentStudentUserId();

            var group = new GroupSpace
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = userId
            };

            await _sharingServices.CreateGroupSpaceAsync(group);

            return RedirectToAction(nameof(MyGroups));
        }

        [HttpGet]
        public IActionResult ShareNote(int noteId)
        {
            var model = new ShareNoteViewModel
            {
                NoteId = noteId,
                TargetType = ShareTargetTypeViewModel.Individual,
                CanEdit = false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareNote(ShareNoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentStudentUserId();

            if (model.TargetType == ShareTargetTypeViewModel.Individual)
            {
                if (!model.TargetUserId.HasValue)
                {
                    ModelState.AddModelError("", "You must provide a user id.");
                    return View(model);
                }

                await _sharingServices.ShareNoteWithUserAsync(
                    model.NoteId,
                    userId,
                    model.TargetUserId.Value,
                    model.CanEdit);
            }
            else
            {
                if (!model.TargetGroupId.HasValue)
                {
                    ModelState.AddModelError("", "You must provide a group id.");
                    return View(model);
                }

                await _sharingServices.ShareNoteWithGroupAsync(
                    model.NoteId,
                    userId,
                    model.TargetGroupId.Value,
                    model.CanEdit);
            }

            return RedirectToAction(nameof(SharedWithMe));
        }

        public async Task<IActionResult> SharedWithMe()
        {
            var userId = GetCurrentStudentUserId();
            var shares = await _sharingServices.GetNotesSharedWithUserAsync(userId);

            var model = shares.Select(s => new SharedNoteListItemViewModel
            {
                SharedNoteId = s.Id,
                NoteId = s.NoteId,
                SharedBy = $"User {s.SharedByUserId}",    
                SharedTo = s.TargetType == ShareTargetType.Group
                    ? $"Group {s.GroupSpaceId}"
                    : $"User {s.SharedWithUserId}",
                CanEdit = s.CanEdit,
                TargetType = s.TargetType.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(NoteCommentInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(SharedWithMe));
            }

            var userId = GetCurrentStudentUserId();

            await _sharingServices.AddCommentAsync(
                model.NoteId,
                userId,
                model.Content,
                model.ParentCommentId);

            return RedirectToAction(nameof(SharedWithMe));
        }

        public async Task<IActionResult> Comments(int noteId)
        {
            var comments = await _sharingServices.GetCommentsForNoteAsync(noteId);

            var model = comments.Select(c => MapCommentToViewModel(c)).ToList();

            return PartialView("_Comments", model); 
        }

        private NoteCommentDisplayViewModel MapCommentToViewModel(NoteComment c)
        {
            return new NoteCommentDisplayViewModel
            {
                Id = c.Id,
                NoteId = c.NoteId,
                AuthorName = $"User {c.AuthorUserId}",   
                Content = c.Content,
                CreatedAt = c.CreatedAt.ToString("g"),
                Replies = c.Replies?.Select(MapCommentToViewModel).ToList()
                            ?? new System.Collections.Generic.List<NoteCommentDisplayViewModel>()
            };
        }
    }
}

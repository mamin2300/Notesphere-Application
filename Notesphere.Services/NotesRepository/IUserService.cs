using Notesphere.Entities.NotesModels;

namespace Notesphere.Services.NotesRepository
{
    public interface IUserService
    {
        Task<StudentUser?> GetByEmailAsync(string email);
        Task<StudentUser?> GetByIdAsync(int id);
        Task<StudentUser> RegisterAsync(string name, string email, string password);
        bool VerifyPassword(StudentUser user, string password);
    }
}

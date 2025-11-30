using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Notesphere.Entities.NotesModels;
using Notesphere.Services.NotesphereDataAccessLayer;

namespace Notesphere.Services.NotesRepository
{
    public class UserRepsoitory : IUserService
    {
        private readonly NotesphereDbContext _db;

        public UserRepsoitory(NotesphereDbContext db)
        {
            _db = db;
        }

        public async Task<StudentUser?> GetByEmailAsync(string email)
        {
            return await _db.StudentUser
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<StudentUser?> GetByIdAsync(int id)
        {
            return await _db.StudentUser.FindAsync(id);
        }

        public async Task<StudentUser> RegisterAsync(string name, string email, string password)
        {
            var existing = await GetByEmailAsync(email);
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            var user = new StudentUser
            {
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            _db.StudentUser.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public bool VerifyPassword(StudentUser user, string password)
        {
            var hash = HashPassword(password);
            return user.PasswordHash == hash;
        }

        private static string HashPassword(string password)
        {
            // basic SHA256 hash – OK for school project
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}

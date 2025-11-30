using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notesphere.Entities.NotesModels
{
    /// Represents a student user who can create and manage notes.
    public class StudentUser
    {
        public int Id { get; set; }  //Primary key for the user.

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;  //Full name of the student.

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(120)]
        public string Email { get; set; } = string.Empty;  // Email address used for login.

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, ErrorMessage = "Password hash is too long.")]
        public string PasswordHash { get; set; } = string.Empty;  //Password hash (NOT plain text).

        [StringLength(50)]
        public string? Username { get; set; }  //Optional username for display or login.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Account creation date.        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Last updated date.
        public ICollection<Note> Notes { get; set; } = new List<Note>();  // Relationship: One student → Many notes.
    }
}

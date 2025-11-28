using System.ComponentModel.DataAnnotations;

namespace SwipSwapAuth.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // Store hashed password, not plain text
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    public class User : AuditableEntity
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public bool IsActive { get; set; } = true;
    }
}



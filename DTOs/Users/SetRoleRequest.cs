using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Users
{
    public class SetRoleRequest
    {
        [Required(ErrorMessage = "Role is required")]
        [RegularExpression(@"^(User|Admin)$", ErrorMessage = "Role must be either 'User' or 'Admin'")]
        public string Role { get; set; } = string.Empty;
    }
}


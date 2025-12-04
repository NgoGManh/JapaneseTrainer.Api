using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Users
{
    public class SetActiveRequest
    {
        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
    }
}


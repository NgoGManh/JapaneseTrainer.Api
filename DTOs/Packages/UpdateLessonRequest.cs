using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class UpdateLessonRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public int Order { get; set; } = 0;
    }
}

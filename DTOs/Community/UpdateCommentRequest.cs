using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Community
{
    public class UpdateCommentRequest
    {
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Range(0, 5)]
        public int Rating { get; set; } = 0;
    }
}

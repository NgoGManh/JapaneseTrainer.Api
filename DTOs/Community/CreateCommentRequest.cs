using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Community
{
    public class CreateCommentRequest
    {
        [Required]
        public Guid PackageId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Range(0, 5)]
        public int Rating { get; set; } = 0;
    }
}

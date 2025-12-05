using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Community
{
    public class CreateReportRequest
    {
        [Required]
        [MaxLength(50)]
        public string TargetType { get; set; } = string.Empty;

        [Required]
        public Guid TargetId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}

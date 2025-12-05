using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Phiên ôn tập (để thống kê số câu trả lời đúng/sai)
    /// </summary>
    public class ReviewSession : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        public int CorrectCount { get; set; } = 0;
        public int TotalAnswered { get; set; } = 0;
    }
}


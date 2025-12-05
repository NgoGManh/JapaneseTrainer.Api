using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Models
{
    public class AIQueue : AuditableEntity
    {
        public Guid Id { get; set; }
        public AIJobType Type { get; set; }
        public string Payload { get; set; } = string.Empty; // JSON string
        public AIJobStatus Status { get; set; } = AIJobStatus.Pending;
        public string? Result { get; set; } // JSON string
        public DateTime? ProcessedAt { get; set; }
    }
}


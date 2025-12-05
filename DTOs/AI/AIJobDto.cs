using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.AI
{
    public class AIJobDto
    {
        public Guid Id { get; set; }
        public AIJobType Type { get; set; }
        public string Payload { get; set; } = string.Empty;
        public AIJobStatus Status { get; set; }
        public string? Result { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}


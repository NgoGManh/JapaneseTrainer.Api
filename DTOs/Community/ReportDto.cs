namespace JapaneseTrainer.Api.DTOs.Community
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public Guid TargetId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

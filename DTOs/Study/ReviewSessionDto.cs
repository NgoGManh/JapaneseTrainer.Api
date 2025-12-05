namespace JapaneseTrainer.Api.DTOs.Study
{
    public class ReviewSessionDto
    {
        public Guid Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int CorrectCount { get; set; }
        public int TotalAnswered { get; set; }
    }
}

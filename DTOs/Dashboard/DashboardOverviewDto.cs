namespace JapaneseTrainer.Api.DTOs.Dashboard
{
    public class DashboardOverviewDto
    {
        public double Accuracy { get; set; }
        public int ReviewsToday { get; set; }
        public int ReviewsDue { get; set; }
        public int StreakDays { get; set; }
        public List<StudyQueueItemShortDto> SrsToday { get; set; } = new();
        public List<DifficultItemDto> DifficultItems { get; set; } = new();
    }
}

using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Dashboard
{
    public class StudyQueueItemShortDto
    {
        public Guid ItemId { get; set; }
        public string Japanese { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public LearningSkill Skill { get; set; }
        public int Stage { get; set; }
        public DateTime? NextReviewAt { get; set; }
    }
}

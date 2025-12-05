using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    public class StudyProgressDto
    {
        public Guid ItemId { get; set; }
        public LearningSkill Skill { get; set; }
        public int Stage { get; set; }
        public DateTime? LastReviewedAt { get; set; }
        public DateTime? NextReviewAt { get; set; }
        public int CorrectStreak { get; set; }
        public int WrongCount { get; set; }
    }
}

using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    public class StudyProgressDto
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? KanjiId { get; set; }
        public string Type { get; set; } = "Item";
        public LearningSkill Skill { get; set; }
        public int Stage { get; set; }
        public DateTime? LastReviewedAt { get; set; }
        public DateTime? NextReviewAt { get; set; }
        public int CorrectStreak { get; set; }
        public int WrongCount { get; set; }
    }
}

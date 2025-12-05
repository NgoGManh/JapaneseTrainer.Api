using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Exercises
{
    public class ExerciseDto
    {
        public Guid Id { get; set; }
        public ExerciseType Type { get; set; }
        public LearningSkill Skill { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string? OptionsJson { get; set; }
        public string? Answer { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? GrammarMasterId { get; set; }
        public Guid? GrammarPackageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Exercises
{
    public class CreateExerciseRequest
    {
        [Required]
        public ExerciseType Type { get; set; }

        [Required]
        public LearningSkill Skill { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Prompt { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? OptionsJson { get; set; }

        [MaxLength(2000)]
        public string? Answer { get; set; }

        public Guid? ItemId { get; set; }
        public Guid? GrammarMasterId { get; set; }
        public Guid? GrammarPackageId { get; set; }
    }
}

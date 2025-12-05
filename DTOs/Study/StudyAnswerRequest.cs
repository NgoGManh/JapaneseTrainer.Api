using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    public class StudyAnswerRequest
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public LearningSkill Skill { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        public Guid? ReviewSessionId { get; set; }
    }
}

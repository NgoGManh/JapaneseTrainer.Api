using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    public class StudyAnswerRequest
    {
        /// <summary>
        /// Item ID (for vocabulary study). One of ItemId or KanjiId must be set.
        /// </summary>
        public Guid? ItemId { get; set; }

        /// <summary>
        /// Kanji ID (for kanji study). One of ItemId or KanjiId must be set.
        /// </summary>
        public Guid? KanjiId { get; set; }

        [Required]
        public LearningSkill Skill { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        public Guid? ReviewSessionId { get; set; }
    }
}

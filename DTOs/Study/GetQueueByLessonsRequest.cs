using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    /// <summary>
    /// Request to get study queue from specific lessons
    /// </summary>
    public class GetQueueByLessonsRequest
    {
        /// <summary>
        /// List of Lesson IDs to study from
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one lesson ID is required")]
        public List<Guid> LessonIds { get; set; } = new();

        /// <summary>
        /// Optional skill filter (Reading, Writing, Listening, Speaking)
        /// </summary>
        public LearningSkill? Skill { get; set; }

        /// <summary>
        /// Maximum number of items to return
        /// </summary>
        [Range(1, 100)]
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Include items from lessons (vocabulary)
        /// </summary>
        public bool IncludeItems { get; set; } = true;

        /// <summary>
        /// Include kanjis from lessons
        /// </summary>
        public bool IncludeKanjis { get; set; } = false;
    }
}


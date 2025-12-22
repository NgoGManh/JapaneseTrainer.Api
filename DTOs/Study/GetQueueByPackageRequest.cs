using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    /// <summary>
    /// Request to get study queue from a Package (all lessons in the package)
    /// </summary>
    public class GetQueueByPackageRequest
    {
        /// <summary>
        /// Package ID to study from
        /// </summary>
        [Required]
        public Guid PackageId { get; set; }

        /// <summary>
        /// Optional: Specific lesson IDs to study from. If empty, includes all lessons in the package.
        /// </summary>
        public List<Guid>? LessonIds { get; set; }

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


using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Exercises
{
    /// <summary>
    /// Filter request for Exercises with pagination
    /// </summary>
    public class ExerciseFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by exercise type
        /// </summary>
        public ExerciseType? Type { get; set; }

        /// <summary>
        /// Filter by learning skill
        /// </summary>
        public LearningSkill? Skill { get; set; }

        /// <summary>
        /// Filter by item ID
        /// </summary>
        public Guid? ItemId { get; set; }

        /// <summary>
        /// Filter by grammar master ID
        /// </summary>
        public Guid? GrammarMasterId { get; set; }

        /// <summary>
        /// Default sort by CreatedAt descending
        /// </summary>
        public new string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Default sort direction descending
        /// </summary>
        public new SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}


using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Community
{
    /// <summary>
    /// Filter request for Comments with pagination
    /// </summary>
    public class CommentFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by item ID
        /// </summary>
        public Guid? ItemId { get; set; }

        /// <summary>
        /// Filter by grammar master ID
        /// </summary>
        public Guid? GrammarMasterId { get; set; }

        /// <summary>
        /// Filter by user ID who created the comment
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Default sort by created_at descending (newest first)
        /// </summary>
        public new string? SortBy { get; set; } = "created_at";

        /// <summary>
        /// Default sort order descending
        /// </summary>
        public new string OrderBy { get; set; } = "desc";
    }
}


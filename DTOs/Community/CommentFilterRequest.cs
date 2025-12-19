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
        /// Default sort by CreatedAt descending (newest first)
        /// </summary>
        public new string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Default sort direction descending
        /// </summary>
        public new SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}


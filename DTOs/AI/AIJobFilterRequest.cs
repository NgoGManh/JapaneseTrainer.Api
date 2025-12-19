using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.AI
{
    /// <summary>
    /// Filter request for AI Jobs with pagination
    /// </summary>
    public class AIJobFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by job type
        /// </summary>
        public AIJobType? Type { get; set; }

        /// <summary>
        /// Filter by job status
        /// </summary>
        public AIJobStatus? Status { get; set; }

        /// <summary>
        /// Filter by user ID who created the job
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Default sort by created_at descending
        /// </summary>
        public new string? SortBy { get; set; } = "created_at";

        /// <summary>
        /// Default sort order descending
        /// </summary>
        public new string OrderBy { get; set; } = "desc";
    }
}


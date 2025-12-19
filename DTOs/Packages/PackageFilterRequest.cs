using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    /// <summary>
    /// Filter request for Packages with pagination
    /// </summary>
    public class PackageFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by owner ID
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Filter by public status
        /// </summary>
        public bool? IsPublic { get; set; }

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


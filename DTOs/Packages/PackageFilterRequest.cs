using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    /// <summary>
    /// Filter request for Packages with pagination
    /// </summary>
    public class PackageFilterRequest : PagedRequest
    {
        /// <summary>
        /// Search term for name or description (partial match)
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filter by owner ID
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Filter by public status
        /// </summary>
        public bool? IsPublic { get; set; }

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


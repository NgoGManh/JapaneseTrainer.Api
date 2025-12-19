using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Users
{
    /// <summary>
    /// Filter request for Users with pagination
    /// </summary>
    public class UserFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by email (partial match)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Filter by username (partial match)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Filter by role (User or Admin)
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Filter by active status
        /// </summary>
        public bool? IsActive { get; set; }

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


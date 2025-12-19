using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Common
{
    /// <summary>
    /// Base class for paginated requests
    /// </summary>
    public class PagedRequest
    {
        /// <summary>
        /// Page number (1-based). Default: 1
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page. Default: 20, Max: 100
        /// </summary>
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Field name to sort by (optional)
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction (Asc or Desc). Default: Asc
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    }
}


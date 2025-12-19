using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Common
{
    /// <summary>
    /// Base class for paginated requests (following Python FastAPI pattern)
    /// </summary>
    public class PagedRequest
    {
        private int _page = 1;
        private int _limit = 20;

        /// <summary>
        /// Page number (1-based). Default: 1
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page. Default: 20, Max: 100
        /// </summary>
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit
        {
            get => _limit;
            set => _limit = value < 1 ? 20 : (value > 100 ? 100 : value);
        }

        /// <summary>
        /// Search term (searches across multiple fields)
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// List of field names to search in (if null, searches in default fields)
        /// </summary>
        public List<string>? SearchIn { get; set; }

        /// <summary>
        /// Field name to sort by. Default: "created_at"
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort order: "asc" or "desc". Default: "desc"
        /// </summary>
        public string OrderBy { get; set; } = "desc";

        /// <summary>
        /// Normalizes pagination values to ensure defaults are applied
        /// </summary>
        public void Normalize()
        {
            if (Page < 1) Page = 1;
            if (Limit < 1) Limit = 20;
            if (Limit > 100) Limit = 100;
            if (string.IsNullOrWhiteSpace(OrderBy) || (OrderBy.ToLower() != "asc" && OrderBy.ToLower() != "desc"))
            {
                OrderBy = "desc";
            }
            else
            {
                OrderBy = OrderBy.ToLower();
            }
        }
    }
}


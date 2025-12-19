using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Common
{
    /// <summary>
    /// Base class for paginated requests
    /// </summary>
    public class PagedRequest
    {
        private int _pageNumber = 1;
        private int _pageSize = 20;

        /// <summary>
        /// Page number (1-based). Default: 1
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page. Default: 20, Max: 100
        /// </summary>
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 20 : (value > 100 ? 100 : value);
        }

        /// <summary>
        /// Field name to sort by (optional)
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction (Asc or Desc). Default: Asc
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Asc;

        /// <summary>
        /// Normalizes pagination values to ensure defaults are applied
        /// </summary>
        public void Normalize()
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize < 1) PageSize = 20;
            if (PageSize > 100) PageSize = 100;
        }
    }
}


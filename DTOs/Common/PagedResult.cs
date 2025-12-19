namespace JapaneseTrainer.Api.DTOs.Common
{
    /// <summary>
    /// Paginated result wrapper
    /// </summary>
    /// <typeparam name="T">Type of items in the result</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// List of items for the current page
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNext => PageNumber < TotalPages;

        /// <summary>
        /// Creates a new empty paged result
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// Creates a new paged result with the specified data
        /// </summary>
        public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}


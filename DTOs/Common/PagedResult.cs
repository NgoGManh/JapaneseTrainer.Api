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
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Limit);

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPrevious => Page > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNext => Page < TotalPages;

        // Backward compatibility properties
        [Obsolete("Use Page instead")]
        public int PageNumber { get => Page; set => Page = value; }

        [Obsolete("Use Limit instead")]
        public int PageSize { get => Limit; set => Limit = value; }

        /// <summary>
        /// Creates a new empty paged result
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// Creates a new paged result with the specified data
        /// </summary>
        public PagedResult(List<T> items, int totalCount, int page, int limit)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            Limit = limit;
        }

    }
}


using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Filter request for Items with pagination
    /// </summary>
    public class ItemFilterRequest : PagedRequest
    {
        /// <summary>
        /// Search term for Japanese, reading, or meaning (partial match)
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filter by item type (Vocabulary, Phrase, Sentence, etc.)
        /// </summary>
        public string? Type { get; set; }

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


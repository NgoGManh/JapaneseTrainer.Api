using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Filter request for Items with pagination
    /// </summary>
    public class ItemFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by item type (Vocabulary, Phrase, Sentence, etc.)
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Default sort by created_at descending (newest first)
        /// </summary>
        public new string? SortBy { get; set; } = "created_at";

        /// <summary>
        /// Default sort order descending
        /// </summary>
        public new string OrderBy { get; set; } = "desc";
    }
}


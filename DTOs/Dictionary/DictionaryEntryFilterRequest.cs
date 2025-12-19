using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Filter request for Dictionary Entries with pagination
    /// </summary>
    public class DictionaryEntryFilterRequest : PagedRequest
    {
        /// <summary>
        /// Search term for Japanese, reading, or meaning (partial match)
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filter by JLPT level (N5, N4, N3, N2, N1)
        /// </summary>
        public string? JlptLevel { get; set; }

        /// <summary>
        /// Filter by Kanji ID
        /// </summary>
        public Guid? KanjiId { get; set; }

        /// <summary>
        /// Filter by part of speech
        /// </summary>
        public string? PartOfSpeech { get; set; }

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


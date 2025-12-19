using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Filter request for Dictionary Entries with pagination
    /// </summary>
    public class DictionaryEntryFilterRequest : PagedRequest
    {
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
        /// Default sort by created_at descending
        /// </summary>
        public new string? SortBy { get; set; } = "created_at";

        /// <summary>
        /// Default sort order descending
        /// </summary>
        public new string OrderBy { get; set; } = "desc";
    }
}


using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Filter request for Kanji with pagination
    /// </summary>
    public class KanjiFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by JLPT level (N5, N4, N3, N2, N1) or grade level
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Filter by minimum stroke count
        /// </summary>
        public int? MinStrokes { get; set; }

        /// <summary>
        /// Filter by maximum stroke count
        /// </summary>
        public int? MaxStrokes { get; set; }

        /// <summary>
        /// Default sort by character ascending
        /// </summary>
        public new string? SortBy { get; set; } = "character";

        /// <summary>
        /// Default sort order ascending
        /// </summary>
        public new string OrderBy { get; set; } = "asc";
    }
}


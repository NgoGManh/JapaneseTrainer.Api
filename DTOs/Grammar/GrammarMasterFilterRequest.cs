using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Grammar
{
    /// <summary>
    /// Filter request for Grammar Masters with pagination
    /// </summary>
    public class GrammarMasterFilterRequest : PagedRequest
    {
        /// <summary>
        /// Filter by JLPT level (N5, N4, N3, N2, N1)
        /// </summary>
        public string? Level { get; set; }

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


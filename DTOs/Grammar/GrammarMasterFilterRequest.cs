using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.DTOs.Grammar
{
    /// <summary>
    /// Filter request for Grammar Masters with pagination
    /// </summary>
    public class GrammarMasterFilterRequest : PagedRequest
    {
        /// <summary>
        /// Search term for pattern, explanation, or example (partial match)
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filter by JLPT level (N5, N4, N3, N2, N1)
        /// </summary>
        public string? Level { get; set; }

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


namespace JapaneseTrainer.Api.DTOs.Common
{
    /// <summary>
    /// Sort direction for ordering results (kept for backward compatibility, but OrderBy string is preferred)
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Ascending order (A-Z, 1-9)
        /// </summary>
        Asc = 0,

        /// <summary>
        /// Descending order (Z-A, 9-1)
        /// </summary>
        Desc = 1
    }

    /// <summary>
    /// Helper class for sort direction conversion
    /// </summary>
    public static class SortDirectionHelper
    {
        public static SortDirection FromString(string orderBy)
        {
            return orderBy?.ToLower() == "asc" ? SortDirection.Asc : SortDirection.Desc;
        }

        public static string ToString(SortDirection direction)
        {
            return direction == SortDirection.Asc ? "asc" : "desc";
        }
    }
}


namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Result of import operation
    /// </summary>
    public class ImportResultDto
    {
        /// <summary>
        /// Number of records successfully updated
        /// </summary>
        public int UpdatedCount { get; set; }

        /// <summary>
        /// Number of records found in Excel but not in database
        /// </summary>
        public int NotFoundCount { get; set; }

        /// <summary>
        /// Total number of records processed from Excel
        /// </summary>
        public int TotalProcessed { get; set; }

        /// <summary>
        /// List of characters/items that were not found in database
        /// </summary>
        public List<string> NotFoundItems { get; set; } = new();

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}


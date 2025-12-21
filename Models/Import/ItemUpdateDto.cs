namespace JapaneseTrainer.Api.Models.Import
{
    /// <summary>
    /// DTO for importing Vietnamese data for Items (vocabulary) from Excel
    /// </summary>
    public class ItemUpdateDto
    {
        /// <summary>
        /// Japanese text (must match Japanese field in database)
        /// </summary>
        public string Japanese { get; set; } = string.Empty;

        /// <summary>
        /// Reading/Kana (must match Reading field in database)
        /// </summary>
        public string? Reading { get; set; }

        /// <summary>
        /// Romaji reading
        /// </summary>
        public string? Romaji { get; set; }

        /// <summary>
        /// Vietnamese meaning (Nghĩa tiếng Việt)
        /// </summary>
        public string? Nghia { get; set; }
    }
}


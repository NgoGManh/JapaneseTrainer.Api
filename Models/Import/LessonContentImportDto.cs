namespace JapaneseTrainer.Api.Models.Import
{
    /// <summary>
    /// DTO for importing lesson content (Items, Kanjis, Grammars) from Excel
    /// </summary>
    public class LessonContentImportDto
    {
        /// <summary>
        /// Type of content: "Item", "Kanji", or "Grammar"
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// For Item: Japanese text (required)
        /// </summary>
        public string? Japanese { get; set; }

        /// <summary>
        /// For Item: Reading/Kana (optional, for better matching)
        /// </summary>
        public string? Reading { get; set; }

        /// <summary>
        /// For Kanji: Character (required)
        /// </summary>
        public string? Character { get; set; }

        /// <summary>
        /// For Grammar: Title (required)
        /// </summary>
        public string? Title { get; set; }
    }
}


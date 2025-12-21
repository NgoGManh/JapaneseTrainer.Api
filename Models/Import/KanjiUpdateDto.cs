namespace JapaneseTrainer.Api.Models.Import
{
    /// <summary>
    /// DTO for importing Vietnamese data for Kanji from Excel
    /// </summary>
    public class KanjiUpdateDto
    {
        /// <summary>
        /// Kanji character (must match Character field in database)
        /// </summary>
        public string Kanji { get; set; } = string.Empty;

        /// <summary>
        /// HanViet reading (âm Hán Việt)
        /// </summary>
        public string? HanViet { get; set; }

        /// <summary>
        /// Vietnamese meaning (Nghĩa tiếng Việt)
        /// </summary>
        public string? Nghia { get; set; }
    }
}


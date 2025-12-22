using Microsoft.AspNetCore.Http;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    /// <summary>
    /// Request wrapper for importing lesson content from Excel file
    /// </summary>
    public class ImportLessonContentFileRequest
    {
        /// <summary>
        /// Excel file containing lesson content (Items, Kanjis, Grammars)
        /// Columns: Type (Item/Kanji/Grammar), Japanese (for Item), Reading (optional for Item), Character (for Kanji), Title (for Grammar)
        /// </summary>
        public IFormFile File { get; set; } = null!;
    }
}


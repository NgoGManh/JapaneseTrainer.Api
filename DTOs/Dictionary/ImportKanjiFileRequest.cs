using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Request model for importing Kanji Vietnamese data from Excel file
    /// </summary>
    public class ImportKanjiFileRequest
    {
        /// <summary>
        /// Excel file containing Kanji, HanViet, and Nghia columns
        /// </summary>
        [SwaggerSchema(Description = "Excel file (.xlsx or .xls) with columns: Kanji, HanViet, Nghia")]
        public IFormFile File { get; set; } = null!;
    }
}


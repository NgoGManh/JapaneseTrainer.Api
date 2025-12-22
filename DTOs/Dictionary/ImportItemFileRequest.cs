using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    /// <summary>
    /// Request model for importing Item Vietnamese data from Excel file
    /// </summary>
    public class ImportItemFileRequest
    {
        /// <summary>
        /// Excel file containing Japanese, Reading (optional), Romaji, and Nghia columns
        /// </summary>
        [SwaggerSchema(Description = "Excel file (.xlsx or .xls) with columns: Japanese, Reading (optional), Romaji, Nghia")]
        public IFormFile File { get; set; } = null!;
    }
}



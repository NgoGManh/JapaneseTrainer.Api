using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Dictionary;
using JapaneseTrainer.Api.Exceptions;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Dictionary management endpoints for vocabulary, kanji, examples, and audio")]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;

        public DictionaryController(IDictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }

        #region Items

        /// <summary>
        /// Get all items with optional filtering
        /// </summary>
        /// <param name="search">Search term for Japanese, reading, or meaning</param>
        /// <param name="type">Filter by item type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of items</returns>
        [HttpGet("items")]
        [SwaggerOperation(
            Summary = "Get all items (paginated)",
            Description = "Retrieves a paginated list of learning items (vocabulary, phrases, etc.) with optional search, type filtering, sorting, and pagination."
        )]
        [SwaggerResponse(200, "Items retrieved successfully", typeof(PagedResult<ItemDto>))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        public async Task<ActionResult<PagedResult<ItemDto>>> GetItems(
            [FromQuery] ItemFilterRequest filter,
            CancellationToken cancellationToken)
        {
            try
            {
                // Ensure filter is normalized
                filter.Normalize();
                var result = await _dictionaryService.GetItemsPagedAsync(filter, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log error for debugging (in production, use ILogger)
                return StatusCode(500, new { 
                    error = "Internal server error", 
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Get item by ID
        /// </summary>
        /// <param name="id">Item ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Item information</returns>
        [HttpGet("items/{id:guid}")]
        [SwaggerOperation(
            Summary = "Get item by ID",
            Description = "Retrieves detailed information about a specific learning item by its ID."
        )]
        [SwaggerResponse(200, "Item found", typeof(ItemDto))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(404, "Item not found")]
        public async Task<ActionResult<ItemDto>> GetItemById(Guid id, CancellationToken cancellationToken)
        {
            var item = await _dictionaryService.GetItemByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        /// <param name="request">Item creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created item</returns>
        [HttpPost("items")]
        [SwaggerOperation(
            Summary = "Create a new item",
            Description = "Creates a new learning item (vocabulary, phrase, etc.) with Japanese text, reading, romaji, and meaning. Automatically checks for duplicates using HashKey."
        )]
        [SwaggerResponse(201, "Item created successfully", typeof(ItemDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(409, "Conflict - Item already exists")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<ItemDto>> CreateItem(
            [FromBody] CreateItemRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var item = await _dictionaryService.CreateItemAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
            }
            catch (DuplicateItemException ex)
            {
                return Conflict(new { 
                    message = ex.Message, 
                    japanese = ex.Japanese, 
                    reading = ex.Reading,
                    hashKey = ex.HashKey 
                });
            }
        }

        /// <summary>
        /// Update an existing item
        /// </summary>
        /// <param name="id">Item ID (GUID)</param>
        /// <param name="request">Item update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated item</returns>
        [HttpPut("items/{id:guid}")]
        [SwaggerOperation(
            Summary = "Update an item",
            Description = "Updates an existing learning item with new information."
        )]
        [SwaggerResponse(200, "Item updated successfully", typeof(ItemDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(404, "Item not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<ItemDto>> UpdateItem(
            Guid id,
            [FromBody] CreateItemRequest request,
            CancellationToken cancellationToken)
        {
            var item = await _dictionaryService.UpdateItemAsync(id, request, cancellationToken);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="id">Item ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        [HttpDelete("items/{id:guid}")]
        [SwaggerOperation(
            Summary = "Delete an item",
            Description = "Deletes a learning item by its ID."
        )]
        [SwaggerResponse(204, "Item deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(404, "Item not found")]
        public async Task<IActionResult> DeleteItem(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _dictionaryService.DeleteItemAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Dictionary Entries

        /// <summary>
        /// Get all dictionary entries with optional filtering (paginated)
        /// </summary>
        [HttpGet("entries")]
        [SwaggerOperation(Summary = "Get all dictionary entries (paginated)", Description = "Retrieves a paginated list of dictionary entries with optional filtering, sorting, and pagination.")]
        [SwaggerResponse(200, "Entries retrieved successfully", typeof(PagedResult<DictionaryEntryDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<ActionResult<PagedResult<DictionaryEntryDto>>> GetDictionaryEntries(
            [FromQuery] DictionaryEntryFilterRequest filter,
            CancellationToken cancellationToken)
        {
            var result = await _dictionaryService.GetDictionaryEntriesPagedAsync(filter, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get dictionary entry by ID
        /// </summary>
        [HttpGet("entries/{id:guid}")]
        [SwaggerOperation(Summary = "Get dictionary entry by ID")]
        [SwaggerResponse(200, "Entry found", typeof(DictionaryEntryDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Entry not found")]
        public async Task<ActionResult<DictionaryEntryDto>> GetDictionaryEntryById(Guid id, CancellationToken cancellationToken)
        {
            var entry = await _dictionaryService.GetDictionaryEntryByIdAsync(id, cancellationToken);
            if (entry == null)
            {
                return NotFound();
            }

            return Ok(entry);
        }

        /// <summary>
        /// Create a new dictionary entry
        /// </summary>
        [HttpPost("entries")]
        [SwaggerOperation(Summary = "Create a new dictionary entry")]
        [SwaggerResponse(201, "Entry created successfully", typeof(DictionaryEntryDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<DictionaryEntryDto>> CreateDictionaryEntry(
            [FromBody] CreateDictionaryEntryRequest request,
            CancellationToken cancellationToken)
        {
            var entry = await _dictionaryService.CreateDictionaryEntryAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetDictionaryEntryById), new { id = entry.Id }, entry);
        }

        /// <summary>
        /// Update a dictionary entry
        /// </summary>
        [HttpPut("entries/{id:guid}")]
        [SwaggerOperation(Summary = "Update a dictionary entry")]
        [SwaggerResponse(200, "Entry updated successfully", typeof(DictionaryEntryDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Entry not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<DictionaryEntryDto>> UpdateDictionaryEntry(
            Guid id,
            [FromBody] CreateDictionaryEntryRequest request,
            CancellationToken cancellationToken)
        {
            var entry = await _dictionaryService.UpdateDictionaryEntryAsync(id, request, cancellationToken);
            if (entry == null)
            {
                return NotFound();
            }

            return Ok(entry);
        }

        /// <summary>
        /// Delete a dictionary entry
        /// </summary>
        [HttpDelete("entries/{id:guid}")]
        [SwaggerOperation(Summary = "Delete a dictionary entry")]
        [SwaggerResponse(204, "Entry deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Entry not found")]
        public async Task<IActionResult> DeleteDictionaryEntry(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _dictionaryService.DeleteDictionaryEntryAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Kanji

        /// <summary>
        /// Get all kanji with optional filtering (paginated)
        /// </summary>
        [HttpGet("kanji")]
        [SwaggerOperation(Summary = "Get all kanji (paginated)", Description = "Retrieves a paginated list of kanji characters with optional filtering, sorting, and pagination.")]
        [SwaggerResponse(200, "Kanji retrieved successfully", typeof(PagedResult<KanjiDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<ActionResult<PagedResult<KanjiDto>>> GetKanjis(
            [FromQuery] KanjiFilterRequest filter,
            CancellationToken cancellationToken)
        {
            var result = await _dictionaryService.GetKanjisPagedAsync(filter, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get kanji by ID
        /// </summary>
        [HttpGet("kanji/{id:guid}")]
        [SwaggerOperation(Summary = "Get kanji by ID")]
        [SwaggerResponse(200, "Kanji found", typeof(KanjiDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Kanji not found")]
        public async Task<ActionResult<KanjiDto>> GetKanjiById(Guid id, CancellationToken cancellationToken)
        {
            var kanji = await _dictionaryService.GetKanjiByIdAsync(id, cancellationToken);
            if (kanji == null)
            {
                return NotFound();
            }

            return Ok(kanji);
        }

        /// <summary>
        /// Create a new kanji
        /// </summary>
        [HttpPost("kanji")]
        [SwaggerOperation(Summary = "Create a new kanji")]
        [SwaggerResponse(201, "Kanji created successfully", typeof(KanjiDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<KanjiDto>> CreateKanji(
            [FromBody] CreateKanjiRequest request,
            CancellationToken cancellationToken)
        {
            var kanji = await _dictionaryService.CreateKanjiAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetKanjiById), new { id = kanji.Id }, kanji);
        }

        /// <summary>
        /// Update a kanji
        /// </summary>
        [HttpPut("kanji/{id:guid}")]
        [SwaggerOperation(Summary = "Update a kanji")]
        [SwaggerResponse(200, "Kanji updated successfully", typeof(KanjiDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Kanji not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<KanjiDto>> UpdateKanji(
            Guid id,
            [FromBody] CreateKanjiRequest request,
            CancellationToken cancellationToken)
        {
            var kanji = await _dictionaryService.UpdateKanjiAsync(id, request, cancellationToken);
            if (kanji == null)
            {
                return NotFound();
            }

            return Ok(kanji);
        }

        /// <summary>
        /// Delete a kanji
        /// </summary>
        [HttpDelete("kanji/{id:guid}")]
        [SwaggerOperation(Summary = "Delete a kanji")]
        [SwaggerResponse(204, "Kanji deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Kanji not found")]
        public async Task<IActionResult> DeleteKanji(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _dictionaryService.DeleteKanjiAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Example Sentences

        /// <summary>
        /// Get all example sentences with optional filtering
        /// </summary>
        [HttpGet("examples")]
        [SwaggerOperation(Summary = "Get all example sentences")]
        [SwaggerResponse(200, "Examples retrieved successfully", typeof(List<ExampleSentenceDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<ActionResult<List<ExampleSentenceDto>>> GetExampleSentences(
            [FromQuery] Guid? itemId,
            [FromQuery] Guid? dictionaryEntryId,
            CancellationToken cancellationToken)
        {
            var examples = await _dictionaryService.GetExampleSentencesAsync(itemId, dictionaryEntryId, cancellationToken);
            return Ok(examples);
        }

        /// <summary>
        /// Get example sentence by ID
        /// </summary>
        [HttpGet("examples/{id:guid}")]
        [SwaggerOperation(Summary = "Get example sentence by ID")]
        [SwaggerResponse(200, "Example found", typeof(ExampleSentenceDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Example not found")]
        public async Task<ActionResult<ExampleSentenceDto>> GetExampleSentenceById(Guid id, CancellationToken cancellationToken)
        {
            var example = await _dictionaryService.GetExampleSentenceByIdAsync(id, cancellationToken);
            if (example == null)
            {
                return NotFound();
            }

            return Ok(example);
        }

        /// <summary>
        /// Create a new example sentence
        /// </summary>
        [HttpPost("examples")]
        [SwaggerOperation(Summary = "Create a new example sentence")]
        [SwaggerResponse(201, "Example created successfully", typeof(ExampleSentenceDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<ExampleSentenceDto>> CreateExampleSentence(
            [FromBody] CreateExampleSentenceRequest request,
            CancellationToken cancellationToken)
        {
            var example = await _dictionaryService.CreateExampleSentenceAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetExampleSentenceById), new { id = example.Id }, example);
        }

        /// <summary>
        /// Update an example sentence
        /// </summary>
        [HttpPut("examples/{id:guid}")]
        [SwaggerOperation(Summary = "Update an example sentence")]
        [SwaggerResponse(200, "Example updated successfully", typeof(ExampleSentenceDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Example not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<ExampleSentenceDto>> UpdateExampleSentence(
            Guid id,
            [FromBody] CreateExampleSentenceRequest request,
            CancellationToken cancellationToken)
        {
            var example = await _dictionaryService.UpdateExampleSentenceAsync(id, request, cancellationToken);
            if (example == null)
            {
                return NotFound();
            }

            return Ok(example);
        }

        /// <summary>
        /// Delete an example sentence
        /// </summary>
        [HttpDelete("examples/{id:guid}")]
        [SwaggerOperation(Summary = "Delete an example sentence")]
        [SwaggerResponse(204, "Example deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Example not found")]
        public async Task<IActionResult> DeleteExampleSentence(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _dictionaryService.DeleteExampleSentenceAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Audio

        /// <summary>
        /// Get all audio files with optional filtering
        /// </summary>
        [HttpGet("audio")]
        [SwaggerOperation(Summary = "Get all audio files")]
        [SwaggerResponse(200, "Audio files retrieved successfully", typeof(List<AudioDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<ActionResult<List<AudioDto>>> GetAudios(
            [FromQuery] Guid? itemId,
            [FromQuery] Guid? dictionaryEntryId,
            CancellationToken cancellationToken)
        {
            var audios = await _dictionaryService.GetAudiosAsync(itemId, dictionaryEntryId, cancellationToken);
            return Ok(audios);
        }

        /// <summary>
        /// Get audio file by ID
        /// </summary>
        [HttpGet("audio/{id:guid}")]
        [SwaggerOperation(Summary = "Get audio file by ID")]
        [SwaggerResponse(200, "Audio found", typeof(AudioDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Audio not found")]
        public async Task<ActionResult<AudioDto>> GetAudioById(Guid id, CancellationToken cancellationToken)
        {
            var audio = await _dictionaryService.GetAudioByIdAsync(id, cancellationToken);
            if (audio == null)
            {
                return NotFound();
            }

            return Ok(audio);
        }

        /// <summary>
        /// Create a new audio file
        /// </summary>
        [HttpPost("audio")]
        [SwaggerOperation(Summary = "Create a new audio file")]
        [SwaggerResponse(201, "Audio created successfully", typeof(AudioDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<AudioDto>> CreateAudio(
            [FromBody] CreateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var audio = await _dictionaryService.CreateAudioAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAudioById), new { id = audio.Id }, audio);
        }

        /// <summary>
        /// Update an audio file
        /// </summary>
        [HttpPut("audio/{id:guid}")]
        [SwaggerOperation(Summary = "Update an audio file")]
        [SwaggerResponse(200, "Audio updated successfully", typeof(AudioDto))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Audio not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<AudioDto>> UpdateAudio(
            Guid id,
            [FromBody] CreateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var audio = await _dictionaryService.UpdateAudioAsync(id, request, cancellationToken);
            if (audio == null)
            {
                return NotFound();
            }

            return Ok(audio);
        }

        /// <summary>
        /// Delete an audio file
        /// </summary>
        [HttpDelete("audio/{id:guid}")]
        [SwaggerOperation(Summary = "Delete an audio file")]
        [SwaggerResponse(204, "Audio deleted successfully")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Audio not found")]
        public async Task<IActionResult> DeleteAudio(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _dictionaryService.DeleteAudioAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Import

        /// <summary>
        /// Import Vietnamese data for Kanji from Excel file
        /// </summary>
        /// <param name="request">Request containing Excel file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Import result with updated count</returns>
        [HttpPost("kanji/import-vietnamese")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Import Vietnamese data for Kanji from Excel",
            Description = "Upload an Excel file to update HanViet and MeaningVietnamese fields for existing Kanji. Only updates fields that are currently null/empty. File format: Excel (.xlsx or .xls) with columns: Kanji, HanViet, Nghia"
        )]
        [SwaggerResponse(200, "Import completed successfully", typeof(ImportResultDto))]
        [SwaggerResponse(400, "Invalid file or empty file")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        public async Task<ActionResult<ImportResultDto>> ImportKanjiVietnamese(
            [FromForm] ImportKanjiFileRequest request,
            CancellationToken cancellationToken)
        {
            if (request?.File == null || request.File.Length == 0)
            {
                return BadRequest(new { error = "Vui lòng upload file Excel" });
            }

            var file = request.File;

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "File phải là định dạng Excel (.xlsx hoặc .xls)" });
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _dictionaryService.ImportKanjiVietnameseAsync(stream, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi xử lý file Excel", message = ex.Message });
            }
        }

        /// <summary>
        /// Import Vietnamese data for Items (vocabulary) from Excel file
        /// </summary>
        /// <param name="request">Request containing Excel file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Import result with updated count</returns>
        [HttpPost("items/import-vietnamese")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Import Vietnamese data for Items from Excel",
            Description = "Upload an Excel file to update Romaji and MeaningVietnamese fields for existing Items. Matches by Japanese + Reading combination. Only updates fields that are currently null/empty. File format: Excel (.xlsx or .xls) with columns: Japanese, Reading (optional), Romaji, Nghia"
        )]
        [SwaggerResponse(200, "Import completed successfully", typeof(ImportResultDto))]
        [SwaggerResponse(400, "Invalid file or empty file")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        public async Task<ActionResult<ImportResultDto>> ImportItemVietnamese(
            [FromForm] ImportItemFileRequest request,
            CancellationToken cancellationToken)
        {
            if (request?.File == null || request.File.Length == 0)
            {
                return BadRequest(new { error = "Vui lòng upload file Excel" });
            }

            var file = request.File;

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "File phải là định dạng Excel (.xlsx hoặc .xls)" });
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _dictionaryService.ImportItemVietnameseAsync(stream, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi xử lý file Excel", message = ex.Message });
            }
        }

        #endregion
    }
}


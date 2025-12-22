using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Packages;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Package & Lesson management (with item/grammar mappings)")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        #region Packages

        [HttpGet]
        [SwaggerOperation(Summary = "Get packages (paginated)", Description = "List packages with optional filters (search, ownerId, isPublic), sorting, and pagination.")]
        [SwaggerResponse(200, "Packages retrieved", typeof(PagedResult<PackageDto>))]
        public async Task<ActionResult<PagedResult<PackageDto>>> GetPackages(
            [FromQuery] PackageFilterRequest filter,
            CancellationToken cancellationToken)
        {
            var result = await _packageService.GetPackagesPagedAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get package by ID")]
        [SwaggerResponse(200, "Package found", typeof(PackageDto))]
        [SwaggerResponse(404, "Package not found")]
        public async Task<ActionResult<PackageDto>> GetPackageById(Guid id, CancellationToken cancellationToken)
        {
            var package = await _packageService.GetPackageByIdAsync(id, cancellationToken);
            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create package")]
        [SwaggerResponse(201, "Package created", typeof(PackageDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<PackageDto>> CreatePackage(
            [FromBody] CreatePackageRequest request,
            CancellationToken cancellationToken)
        {
            var package = await _packageService.CreatePackageAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update package")]
        [SwaggerResponse(200, "Package updated", typeof(PackageDto))]
        [SwaggerResponse(404, "Package not found")]
        public async Task<ActionResult<PackageDto>> UpdatePackage(
            Guid id,
            [FromBody] UpdatePackageRequest request,
            CancellationToken cancellationToken)
        {
            var updated = await _packageService.UpdatePackageAsync(id, request, cancellationToken);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete package")]
        [SwaggerResponse(204, "Package deleted")]
        [SwaggerResponse(404, "Package not found")]
        public async Task<IActionResult> DeletePackage(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _packageService.DeletePackageAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Lessons

        [HttpGet("{packageId:guid}/lessons")]
        [SwaggerOperation(Summary = "Get lessons of a package")]
        [SwaggerResponse(200, "Lessons retrieved", typeof(List<LessonDto>))]
        [SwaggerResponse(404, "Package not found")]
        public async Task<ActionResult<List<LessonDto>>> GetLessons(Guid packageId, CancellationToken cancellationToken)
        {
            // Note: service will return empty list if package exists but no lessons; if package not found, we can still return empty
            var lessons = await _packageService.GetLessonsAsync(packageId, cancellationToken);
            return Ok(lessons);
        }

        [HttpGet("lessons/{id:guid}")]
        [SwaggerOperation(Summary = "Get lesson by ID")]
        [SwaggerResponse(200, "Lesson found", typeof(LessonDto))]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> GetLessonById(Guid id, CancellationToken cancellationToken)
        {
            var lesson = await _packageService.GetLessonByIdAsync(id, cancellationToken);
            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        [HttpPost("lessons")]
        [SwaggerOperation(Summary = "Create lesson")]
        [SwaggerResponse(201, "Lesson created", typeof(LessonDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<LessonDto>> CreateLesson(
            [FromBody] CreateLessonRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var lesson = await _packageService.CreateLessonAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("lessons/{id:guid}")]
        [SwaggerOperation(Summary = "Update lesson")]
        [SwaggerResponse(200, "Lesson updated", typeof(LessonDto))]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(
            Guid id,
            [FromBody] UpdateLessonRequest request,
            CancellationToken cancellationToken)
        {
            var lesson = await _packageService.UpdateLessonAsync(id, request, cancellationToken);
            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        [HttpDelete("lessons/{id:guid}")]
        [SwaggerOperation(Summary = "Delete lesson")]
        [SwaggerResponse(204, "Lesson deleted")]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<IActionResult> DeleteLesson(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _packageService.DeleteLessonAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region Lesson Items

        [HttpPost("lessons/{id:guid}/items")]
        [SwaggerOperation(Summary = "Add item to lesson")]
        [SwaggerResponse(200, "Item added", typeof(LessonDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> AddLessonItem(
            Guid id,
            [FromBody] AddLessonItemRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var lesson = await _packageService.AddLessonItemAsync(id, request.ItemId, cancellationToken);
                if (lesson == null)
                {
                    return NotFound();
                }
                return Ok(lesson);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("lessons/{lessonId:guid}/items/{itemId:guid}")]
        [SwaggerOperation(Summary = "Remove item from lesson")]
        [SwaggerResponse(200, "Item removed", typeof(LessonDto))]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> RemoveLessonItem(
            Guid lessonId,
            Guid itemId,
            CancellationToken cancellationToken)
        {
            var lesson = await _packageService.RemoveLessonItemAsync(lessonId, itemId, cancellationToken);
            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        #endregion

        #region Lesson Grammar

        [HttpPost("lessons/{id:guid}/grammars")]
        [SwaggerOperation(Summary = "Add grammar to lesson")]
        [SwaggerResponse(200, "Grammar added", typeof(LessonDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> AddLessonGrammar(
            Guid id,
            [FromBody] AddLessonGrammarRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var lesson = await _packageService.AddLessonGrammarAsync(id, request.GrammarMasterId, request.GrammarPackageId, cancellationToken);
                if (lesson == null)
                {
                    return NotFound();
                }

                return Ok(lesson);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("lessons/{lessonId:guid}/grammars/{grammarMasterId:guid}")]
        [SwaggerOperation(Summary = "Remove grammar from lesson")]
        [SwaggerResponse(200, "Grammar removed", typeof(LessonDto))]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> RemoveLessonGrammar(
            Guid lessonId,
            Guid grammarMasterId,
            CancellationToken cancellationToken)
        {
            var lesson = await _packageService.RemoveLessonGrammarAsync(lessonId, grammarMasterId, cancellationToken);
            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        #endregion

        #region Lesson Kanji

        [HttpPost("lessons/{id:guid}/kanjis")]
        [SwaggerOperation(Summary = "Add kanji to lesson")]
        [SwaggerResponse(200, "Kanji added", typeof(LessonDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> AddLessonKanji(
            Guid id,
            [FromBody] AddLessonKanjiRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var lesson = await _packageService.AddLessonKanjiAsync(id, request.KanjiId, cancellationToken);
                if (lesson == null)
                {
                    return NotFound();
                }

                return Ok(lesson);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("lessons/{lessonId:guid}/kanjis/{kanjiId:guid}")]
        [SwaggerOperation(Summary = "Remove kanji from lesson")]
        [SwaggerResponse(200, "Kanji removed", typeof(LessonDto))]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonDto>> RemoveLessonKanji(
            Guid lessonId,
            Guid kanjiId,
            CancellationToken cancellationToken)
        {
            var lesson = await _packageService.RemoveLessonKanjiAsync(lessonId, kanjiId, cancellationToken);
            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        #endregion

        #region Lesson Import

        [HttpPost("lessons/{id:guid}/import")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Import lesson content from Excel", Description = "Import Items, Kanjis, and Grammars to a lesson from Excel file. Excel columns: Type (Item/Kanji/Grammar), Japanese (for Item), Reading (optional for Item), Character (for Kanji), Title (for Grammar)")]
        [SwaggerResponse(200, "Import completed", typeof(LessonImportResultDto))]
        [SwaggerResponse(400, "Invalid request")]
        [SwaggerResponse(404, "Lesson not found")]
        public async Task<ActionResult<LessonImportResultDto>> ImportLessonContent(
            Guid id,
            [FromForm] ImportLessonContentFileRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest(new { message = "File không được để trống" });
                }

                // Validate file extension
                var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    return BadRequest(new { message = "Chỉ chấp nhận file Excel (.xlsx, .xls)" });
                }

                using var stream = request.File.OpenReadStream();
                var result = await _packageService.ImportLessonContentAsync(id, stream, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi import: {ex.Message}" });
            }
        }

        #endregion
    }
}

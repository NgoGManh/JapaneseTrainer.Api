using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Grammar;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Grammar endpoints: master definitions and package-specific overrides")]
    public class GrammarController : ControllerBase
    {
        private readonly IGrammarService _grammarService;

        public GrammarController(IGrammarService grammarService)
        {
            _grammarService = grammarService;
        }

        #region Grammar Masters

        [HttpGet("masters")]
        [SwaggerOperation(Summary = "Get grammar masters", Description = "List all grammar master records with optional search and level filter.")]
        [SwaggerResponse(200, "Masters retrieved", typeof(List<GrammarMasterDto>))]
        public async Task<ActionResult<List<GrammarMasterDto>>> GetMasters(
            [FromQuery] string? search,
            [FromQuery] string? level,
            CancellationToken cancellationToken)
        {
            var masters = await _grammarService.GetMastersAsync(search, level, cancellationToken);
            return Ok(masters);
        }

        [HttpGet("masters/{id:guid}")]
        [SwaggerOperation(Summary = "Get grammar master by ID")]
        [SwaggerResponse(200, "Master found", typeof(GrammarMasterDto))]
        [SwaggerResponse(404, "Master not found")]
        public async Task<ActionResult<GrammarMasterDto>> GetMasterById(Guid id, CancellationToken cancellationToken)
        {
            var master = await _grammarService.GetMasterByIdAsync(id, cancellationToken);
            if (master == null)
            {
                return NotFound();
            }

            return Ok(master);
        }

        [HttpPost("masters")]
        [SwaggerOperation(Summary = "Create grammar master")]
        [SwaggerResponse(201, "Master created", typeof(GrammarMasterDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<GrammarMasterDto>> CreateMaster(
            [FromBody] CreateGrammarMasterRequest request,
            CancellationToken cancellationToken)
        {
            var master = await _grammarService.CreateMasterAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetMasterById), new { id = master.Id }, master);
        }

        [HttpPut("masters/{id:guid}")]
        [SwaggerOperation(Summary = "Update grammar master")]
        [SwaggerResponse(200, "Master updated", typeof(GrammarMasterDto))]
        [SwaggerResponse(404, "Master not found")]
        public async Task<ActionResult<GrammarMasterDto>> UpdateMaster(
            Guid id,
            [FromBody] UpdateGrammarMasterRequest request,
            CancellationToken cancellationToken)
        {
            var updated = await _grammarService.UpdateMasterAsync(id, request, cancellationToken);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("masters/{id:guid}")]
        [SwaggerOperation(Summary = "Delete grammar master")]
        [SwaggerResponse(204, "Master deleted")]
        [SwaggerResponse(400, "Cannot delete master linked to package")]
        [SwaggerResponse(404, "Master not found")]
        public async Task<IActionResult> DeleteMaster(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await _grammarService.DeleteMasterAsync(id, cancellationToken);
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Grammar Packages

        [HttpGet("packages")]
        [SwaggerOperation(Summary = "Get grammar packages", Description = "List grammar packages with optional filters by masterId or packageId.")]
        [SwaggerResponse(200, "Packages retrieved", typeof(List<GrammarPackageDto>))]
        public async Task<ActionResult<List<GrammarPackageDto>>> GetPackages(
            [FromQuery] Guid? masterId,
            [FromQuery] Guid? packageId,
            CancellationToken cancellationToken)
        {
            var packages = await _grammarService.GetPackagesAsync(masterId, packageId, cancellationToken);
            return Ok(packages);
        }

        [HttpGet("packages/{id:guid}")]
        [SwaggerOperation(Summary = "Get grammar package by ID")]
        [SwaggerResponse(200, "Package found", typeof(GrammarPackageDto))]
        [SwaggerResponse(404, "Package not found")]
        public async Task<ActionResult<GrammarPackageDto>> GetPackageById(Guid id, CancellationToken cancellationToken)
        {
            var package = await _grammarService.GetPackageByIdAsync(id, cancellationToken);
            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        [HttpPost("packages")]
        [SwaggerOperation(Summary = "Create grammar package")]
        [SwaggerResponse(201, "Package created", typeof(GrammarPackageDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<GrammarPackageDto>> CreatePackage(
            [FromBody] CreateGrammarPackageRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var package = await _grammarService.CreatePackageAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("packages/{id:guid}")]
        [SwaggerOperation(Summary = "Update grammar package")]
        [SwaggerResponse(200, "Package updated", typeof(GrammarPackageDto))]
        [SwaggerResponse(404, "Package not found")]
        public async Task<ActionResult<GrammarPackageDto>> UpdatePackage(
            Guid id,
            [FromBody] UpdateGrammarPackageRequest request,
            CancellationToken cancellationToken)
        {
            var package = await _grammarService.UpdatePackageAsync(id, request, cancellationToken);
            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        [HttpDelete("packages/{id:guid}")]
        [SwaggerOperation(Summary = "Delete grammar package")]
        [SwaggerResponse(204, "Package deleted")]
        [SwaggerResponse(404, "Package not found")]
        public async Task<IActionResult> DeletePackage(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _grammarService.DeletePackageAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion
    }
}

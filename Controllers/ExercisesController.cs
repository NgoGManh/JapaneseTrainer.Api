using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Exercises;
using JapaneseTrainer.Api.Models.Enums;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Exercise endpoints")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get exercises", Description = "Filter by type, skill, itemId, grammarMasterId")]
        [SwaggerResponse(200, "Exercises retrieved", typeof(List<ExerciseDto>))]
        public async Task<ActionResult<List<ExerciseDto>>> GetExercises(
            [FromQuery] ExerciseType? type,
            [FromQuery] LearningSkill? skill,
            [FromQuery] Guid? itemId,
            [FromQuery] Guid? grammarMasterId,
            CancellationToken cancellationToken)
        {
            var list = await _exerciseService.GetExercisesAsync(type, skill, itemId, grammarMasterId, cancellationToken);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get exercise by ID")]
        [SwaggerResponse(200, "Exercise found", typeof(ExerciseDto))]
        [SwaggerResponse(404, "Exercise not found")]
        public async Task<ActionResult<ExerciseDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _exerciseService.GetByIdAsync(id, cancellationToken);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create exercise")]
        [SwaggerResponse(201, "Exercise created", typeof(ExerciseDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<ExerciseDto>> Create(
            [FromBody] CreateExerciseRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var dto = await _exerciseService.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update exercise")]
        [SwaggerResponse(200, "Exercise updated", typeof(ExerciseDto))]
        [SwaggerResponse(404, "Exercise not found")]
        public async Task<ActionResult<ExerciseDto>> Update(
            Guid id,
            [FromBody] UpdateExerciseRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var dto = await _exerciseService.UpdateAsync(id, request, cancellationToken);
                if (dto == null) return NotFound();
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete exercise")]
        [SwaggerResponse(204, "Exercise deleted")]
        [SwaggerResponse(404, "Exercise not found")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var ok = await _exerciseService.DeleteAsync(id, cancellationToken);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

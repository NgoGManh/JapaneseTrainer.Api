using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.AI;
using JapaneseTrainer.Api.Models.Enums;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("AI Queue endpoints (stub)")]
    public class AIController : ControllerBase
    {
        private readonly IAIQueueService _aiQueueService;

        public AIController(IAIQueueService aiQueueService)
        {
            _aiQueueService = aiQueueService;
        }

        [HttpPost("jobs")]
        [SwaggerOperation(Summary = "Create AI job", Description = "Submit a new AI job to the queue (OCR, QUIZ_GEN, GRAMMAR_EXPLAIN, EXERCISE_GEN, TRANSLATION, AUDIO_GEN)")]
        [SwaggerResponse(201, "Job created", typeof(AIJobDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<AIJobDto>> CreateJob(
            [FromBody] AIJobRequest request,
            CancellationToken cancellationToken)
        {
            var dto = await _aiQueueService.CreateJobAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetJobById), new { id = dto.Id }, dto);
        }

        [HttpGet("jobs")]
        [SwaggerOperation(Summary = "Get AI jobs", Description = "List jobs with optional filters (type, status)")]
        [SwaggerResponse(200, "Jobs retrieved", typeof(List<AIJobDto>))]
        public async Task<ActionResult<List<AIJobDto>>> GetJobs(
            [FromQuery] AIJobType? type,
            [FromQuery] AIJobStatus? status,
            CancellationToken cancellationToken)
        {
            var list = await _aiQueueService.GetJobsAsync(type, status, cancellationToken);
            return Ok(list);
        }

        [HttpGet("jobs/{id:guid}")]
        [SwaggerOperation(Summary = "Get AI job by ID")]
        [SwaggerResponse(200, "Job found", typeof(AIJobDto))]
        [SwaggerResponse(404, "Job not found")]
        public async Task<ActionResult<AIJobDto>> GetJobById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _aiQueueService.GetJobByIdAsync(id, cancellationToken);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("jobs/{id:guid}/process")]
        [SwaggerOperation(Summary = "Process AI job (mock)", Description = "Mock processing of an AI job. In production, this would be handled by background workers.")]
        [SwaggerResponse(200, "Job processed", typeof(AIJobDto))]
        [SwaggerResponse(404, "Job not found")]
        public async Task<ActionResult<AIJobDto>> ProcessJob(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _aiQueueService.ProcessJobAsync(id, cancellationToken);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}


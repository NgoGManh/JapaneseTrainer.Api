using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Study;
using JapaneseTrainer.Api.Models.Enums;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Study/SRS endpoints")]
    public class StudyController : ControllerBase
    {
        private readonly IStudyService _studyService;

        public StudyController(IStudyService studyService)
        {
            _studyService = studyService;
        }

        [HttpGet("queue")]
        [SwaggerOperation(Summary = "Get review queue", Description = "Return due items for the user by skill.")]
        [SwaggerResponse(200, "Queue returned", typeof(List<StudyQueueItemDto>))]
        public async Task<ActionResult<List<StudyQueueItemDto>>> GetQueue(
            [FromQuery] LearningSkill? skill,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var queue = await _studyService.GetQueueAsync(userId, skill, limit, cancellationToken);
            return Ok(queue);
        }

        [HttpPost("answer")]
        [SwaggerOperation(Summary = "Submit study answer", Description = "Update SRS progress for an item.")]
        [SwaggerResponse(200, "Progress updated", typeof(StudyProgressDto))]
        public async Task<ActionResult<StudyProgressDto>> SubmitAnswer(
            [FromBody] StudyAnswerRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var progress = await _studyService.SubmitAnswerAsync(userId, request, cancellationToken);
            return Ok(progress);
        }

        [HttpPost("session/start")]
        [SwaggerOperation(Summary = "Start review session")]
        [SwaggerResponse(200, "Session started", typeof(ReviewSessionDto))]
        public async Task<ActionResult<ReviewSessionDto>> StartSession(CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var session = await _studyService.StartSessionAsync(userId, cancellationToken);
            return Ok(session);
        }

        [HttpPost("session/{id:guid}/end")]
        [SwaggerOperation(Summary = "End review session")]
        [SwaggerResponse(200, "Session ended", typeof(ReviewSessionDto))]
        [SwaggerResponse(404, "Session not found")]
        public async Task<ActionResult<ReviewSessionDto>> EndSession(
            Guid id,
            [FromQuery] int correctCount,
            [FromQuery] int totalAnswered,
            CancellationToken cancellationToken = default)
        {
            var session = await _studyService.EndSessionAsync(id, correctCount, totalAnswered, cancellationToken);
            if (session == null) return NotFound();
            return Ok(session);
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var userId)) return userId;
            throw new UnauthorizedAccessException("Invalid user id");
        }
    }
}

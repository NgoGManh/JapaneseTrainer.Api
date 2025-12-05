using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Community;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Community endpoints: comments & reports")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        #region Comments

        [HttpGet("comments")]
        [SwaggerOperation(Summary = "Get comments", Description = "Filter by packageId or userId")]
        [SwaggerResponse(200, "Comments retrieved", typeof(List<CommentDto>))]
        public async Task<ActionResult<List<CommentDto>>> GetComments(
            [FromQuery] Guid? packageId,
            [FromQuery] Guid? userId,
            CancellationToken cancellationToken)
        {
            var list = await _communityService.GetCommentsAsync(packageId, userId, cancellationToken);
            return Ok(list);
        }

        [HttpGet("comments/{id:guid}")]
        [SwaggerOperation(Summary = "Get comment by ID")]
        [SwaggerResponse(200, "Comment found", typeof(CommentDto))]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<ActionResult<CommentDto>> GetCommentById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _communityService.GetCommentByIdAsync(id, cancellationToken);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("comments")]
        [SwaggerOperation(Summary = "Create comment")]
        [SwaggerResponse(201, "Comment created", typeof(CommentDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<CommentDto>> CreateComment(
            [FromBody] CreateCommentRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserId();
                var dto = await _communityService.CreateCommentAsync(userId, request, cancellationToken);
                return CreatedAtAction(nameof(GetCommentById), new { id = dto.Id }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("comments/{id:guid}")]
        [SwaggerOperation(Summary = "Update comment")]
        [SwaggerResponse(200, "Comment updated", typeof(CommentDto))]
        [SwaggerResponse(401, "Cannot edit others' comments")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<ActionResult<CommentDto>> UpdateComment(
            Guid id,
            [FromBody] UpdateCommentRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserId();
                var dto = await _communityService.UpdateCommentAsync(id, userId, request, cancellationToken);
                if (dto == null) return NotFound();
                return Ok(dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete("comments/{id:guid}")]
        [SwaggerOperation(Summary = "Delete comment")]
        [SwaggerResponse(204, "Comment deleted")]
        [SwaggerResponse(401, "Cannot delete others' comments")]
        [SwaggerResponse(404, "Comment not found")]
        public async Task<IActionResult> DeleteComment(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserId();
                var deleted = await _communityService.DeleteCommentAsync(id, userId, cancellationToken);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        #endregion

        #region Reports

        [HttpPost("reports")]
        [SwaggerOperation(Summary = "Create report")]
        [SwaggerResponse(201, "Report created", typeof(ReportDto))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<ActionResult<ReportDto>> CreateReport(
            [FromBody] CreateReportRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var dto = await _communityService.CreateReportAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetReportById), new { id = dto.Id }, dto);
        }

        [HttpGet("reports")]
        [SwaggerOperation(Summary = "Get reports", Description = "Admin-only suggested; filter by status")]
        [SwaggerResponse(200, "Reports retrieved", typeof(List<ReportDto>))]
        public async Task<ActionResult<List<ReportDto>>> GetReports(
            [FromQuery] string? status,
            CancellationToken cancellationToken)
        {
            var list = await _communityService.GetReportsAsync(status, cancellationToken);
            return Ok(list);
        }

        [HttpGet("reports/{id:guid}")]
        [SwaggerOperation(Summary = "Get report by ID")]
        [SwaggerResponse(200, "Report found", typeof(ReportDto))]
        [SwaggerResponse(404, "Report not found")]
        public async Task<ActionResult<ReportDto>> GetReportById(Guid id, CancellationToken cancellationToken)
        {
            var report = await _communityService.GetReportsAsync(null, cancellationToken);
            var dto = report.FirstOrDefault(r => r.Id == id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("reports/{id:guid}/status")]
        [SwaggerOperation(Summary = "Update report status")]
        [SwaggerResponse(200, "Report updated", typeof(ReportDto))]
        [SwaggerResponse(404, "Report not found")]
        public async Task<ActionResult<ReportDto>> UpdateReportStatus(
            Guid id,
            [FromQuery] string status,
            CancellationToken cancellationToken)
        {
            var dto = await _communityService.UpdateReportStatusAsync(id, status, cancellationToken);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        #endregion

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var userId)) return userId;
            throw new UnauthorizedAccessException("Invalid user id");
        }
    }
}

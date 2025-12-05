using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Dashboard;
using JapaneseTrainer.Api.Models.Enums;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    [SwaggerTag("Dashboard metrics endpoints")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        [SwaggerOperation(Summary = "Get dashboard overview", Description = "Returns accuracy, reviews today/due, streak, SRS queue today, difficult items.")]
        [SwaggerResponse(200, "Overview returned", typeof(DashboardOverviewDto))]
        public async Task<ActionResult<DashboardOverviewDto>> GetOverview(
            [FromQuery] LearningSkill? skill,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var dto = await _dashboardService.GetOverviewAsync(userId, skill, cancellationToken);
            return Ok(dto);
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(sub, out var userId)) return userId;
            throw new UnauthorizedAccessException("Invalid user id");
        }
    }
}

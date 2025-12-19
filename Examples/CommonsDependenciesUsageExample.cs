using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JapaneseTrainer.Api.Common;
using JapaneseTrainer.Api.DTOs.Common;

namespace JapaneseTrainer.Api.Examples
{
    /// <summary>
    /// Example controller showing how to use CommonsDependencies
    /// Similar to Python FastAPI pattern
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    public class ExampleController : ControllerBase
    {
        private readonly ICommonsDependenciesService _commonsService;

        public ExampleController(ICommonsDependenciesService commonsService)
        {
            _commonsService = commonsService;
        }

        /// <summary>
        /// Example endpoint using CommonsDependencies
        /// </summary>
        [HttpGet("example")]
        public IActionResult GetExample([FromQuery] PagedRequest pagination)
        {
            var commons = _commonsService.GetCommonsDependencies();

            // Access current user information
            if (commons.IsPublicApi)
            {
                return Ok(new { message = "This is a public API call" });
            }

            var userId = commons.GetCurrentUserId(); // Throws if not authenticated
            var userType = commons.UserType;

            return Ok(new
            {
                userId,
                userType,
                isPublicApi = commons.IsPublicApi,
                page = pagination.Page,
                limit = pagination.Limit
            });
        }

        /// <summary>
        /// Alternative: Direct usage without service injection
        /// </summary>
        [HttpGet("example-direct")]
        public IActionResult GetExampleDirect([FromQuery] PagedRequest pagination)
        {
            var commons = new CommonsDependencies(this); // Direct from ControllerBase

            if (commons.IsPublicApi)
            {
                return Unauthorized();
            }

            var userId = commons.GetCurrentUserId();
            return Ok(new { userId, page = pagination.Page });
        }
    }
}


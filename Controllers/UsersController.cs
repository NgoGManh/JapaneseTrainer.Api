using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JapaneseTrainer.Api.DTOs.Users;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll(
            [FromQuery] string? email,
            [FromQuery] string? username,
            [FromQuery] string? role,
            [FromQuery] bool? isActive,
            CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(email, username, role, isActive, cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("{id:guid}/role")]
        public async Task<IActionResult> SetRole(Guid id, [FromQuery] string role, CancellationToken cancellationToken)
        {
            var ok = await _userService.SetRoleAsync(id, role, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { message = "Invalid user or role" });
            }

            return NoContent();
        }

        [HttpPost("{id:guid}/active")]
        public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive, CancellationToken cancellationToken)
        {
            var ok = await _userService.SetActiveAsync(id, isActive, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { message = "Invalid user" });
            }

            return NoContent();
        }
    }
}



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Users;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(Roles = "Admin")]
    [SwaggerTag("User management endpoints (Admin only)")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users with optional filtering
        /// </summary>
        /// <param name="email">Filter by email (partial match)</param>
        /// <param name="username">Filter by username (partial match)</param>
        /// <param name="role">Filter by role (User or Admin)</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users matching the filters</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Retrieves a list of all users with optional filtering by email, username, role, and active status. Requires Admin role."
        )]
        [SwaggerResponse(200, "Users retrieved successfully", typeof(List<UserDto>))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(403, "Forbidden - Admin role required")]
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

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User information</returns>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get user by ID",
            Description = "Retrieves detailed information about a specific user by their ID. Requires Admin role."
        )]
        [SwaggerResponse(200, "User found", typeof(UserDto))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(403, "Forbidden - Admin role required")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Update user role
        /// </summary>
        /// <param name="id">User ID (GUID)</param>
        /// <param name="request">Role update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        [HttpPost("{id:guid}/role")]
        [SwaggerOperation(
            Summary = "Update user role",
            Description = "Updates the role of a specific user. Role must be either 'User' or 'Admin'. Requires Admin role."
        )]
        [SwaggerResponse(204, "Role updated successfully")]
        [SwaggerResponse(400, "Invalid user or role")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(403, "Forbidden - Admin role required")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<IActionResult> SetRole(
            Guid id, 
            [FromBody] SetRoleRequest request, 
            CancellationToken cancellationToken)
        {
            var ok = await _userService.SetRoleAsync(id, request.Role, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { message = "Invalid user or role" });
            }

            return NoContent();
        }

        /// <summary>
        /// Update user active status
        /// </summary>
        /// <param name="id">User ID (GUID)</param>
        /// <param name="request">Active status update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        [HttpPost("{id:guid}/active")]
        [SwaggerOperation(
            Summary = "Update user active status",
            Description = "Activates or deactivates a user account. Inactive users cannot login. Requires Admin role."
        )]
        [SwaggerResponse(204, "Active status updated successfully")]
        [SwaggerResponse(400, "Invalid user")]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(403, "Forbidden - Admin role required")]
        [SwaggerResponse(404, "User not found")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<IActionResult> SetActive(
            Guid id, 
            [FromBody] SetActiveRequest request, 
            CancellationToken cancellationToken)
        {
            var ok = await _userService.SetActiveAsync(id, request.IsActive, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { message = "Invalid user" });
            }

            return NoContent();
        }
    }
}



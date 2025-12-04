using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using JapaneseTrainer.Api.DTOs.Auth;
using JapaneseTrainer.Api.DTOs.Users;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [SwaggerTag("Authentication endpoints for user registration, login, and profile management")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _authService = authService;
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">Registration information including email, password, and optional username</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account with the provided email and password. Returns a JWT token for immediate authentication."
        )]
        [SwaggerResponse(200, "Registration successful", typeof(AuthResponse))]
        [SwaggerResponse(400, "Invalid request or email already exists")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<AuthResponse>> Register(
            [FromBody] RegisterRequest request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.RegisterAsync(request, cancellationToken);
                var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAt);

                return Ok(new AuthResponse
                {
                    AccessToken = token,
                    ExpiresAt = expiresAt,
                    UserId = user.Id,
                    Username = user.Username ?? string.Empty,
                    Email = user.Email,
                    Role = user.Role.ToString()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Login with email and password",
            Description = "Authenticates a user with email and password. Returns a JWT token for subsequent API requests."
        )]
        [SwaggerResponse(200, "Login successful", typeof(AuthResponse))]
        [SwaggerResponse(401, "Invalid email or password")]
        [SwaggerResponse(422, "Validation error")]
        public async Task<ActionResult<AuthResponse>> Login(
            [FromBody] LoginRequest request, 
            CancellationToken cancellationToken)
        {
            var user = await _authService.ValidateUserAsync(request, cancellationToken);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAt);

            return Ok(new AuthResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Username = user.Username ?? string.Empty,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// Get current authenticated user information
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get current user profile",
            Description = "Returns the profile information of the currently authenticated user. Requires a valid JWT token."
        )]
        [SwaggerResponse(200, "User information retrieved successfully", typeof(UserDto))]
        [SwaggerResponse(401, "Unauthorized - Invalid or missing token")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<UserDto>> Me(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}



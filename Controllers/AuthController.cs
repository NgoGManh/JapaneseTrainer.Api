using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JapaneseTrainer.Api.DTOs.Auth;
using JapaneseTrainer.Api.DTOs.Users;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Services;

namespace JapaneseTrainer.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
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
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
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
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }

        [HttpGet("me")]
        [Authorize]
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



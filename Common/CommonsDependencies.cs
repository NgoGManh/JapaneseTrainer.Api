using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JapaneseTrainer.Api.Common
{
    /// <summary>
    /// Handles common dependencies extracted from the request.
    /// Similar to CommonsDependencies in Python FastAPI
    /// </summary>
    public class CommonsDependencies
    {
        /// <summary>
        /// The ID of the current user extracted from JWT claims
        /// </summary>
        public Guid? CurrentUserId { get; private set; }

        /// <summary>
        /// The type/role of the current user (e.g., Admin, User) extracted from JWT claims
        /// </summary>
        public string? UserType { get; private set; }

        /// <summary>
        /// Indicates whether the request is from a public API (no authentication required)
        /// </summary>
        public bool IsPublicApi { get; private set; }

        /// <summary>
        /// Creates CommonsDependencies from HttpContext (for use in controllers)
        /// </summary>
        public CommonsDependencies(HttpContext httpContext)
        {
            var user = httpContext.User;
            
            if (user?.Identity?.IsAuthenticated == true)
            {
                // Extract user ID from claims
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? user.FindFirst("sub")?.Value;
                
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    CurrentUserId = userId;
                }

                // Extract user type/role from claims
                UserType = user.FindFirst(ClaimTypes.Role)?.Value;
                IsPublicApi = false;
            }
            else
            {
                CurrentUserId = null;
                UserType = null;
                IsPublicApi = true;
            }
        }

        /// <summary>
        /// Creates CommonsDependencies from ControllerBase (for use in controllers)
        /// </summary>
        public CommonsDependencies(ControllerBase controller)
            : this(controller.HttpContext)
        {
        }

        /// <summary>
        /// Gets the current user ID or throws UnauthorizedAccessException if not authenticated
        /// </summary>
        public Guid GetCurrentUserId()
        {
            if (!CurrentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return CurrentUserId.Value;
        }
    }
}

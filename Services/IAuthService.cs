using JapaneseTrainer.Api.DTOs.Auth;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<User?> ValidateUserAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}



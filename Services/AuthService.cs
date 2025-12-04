using System.Security.Cryptography;
using System.Text;
using JapaneseTrainer.Api.DTOs.Auth;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Repositories;

namespace JapaneseTrainer.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existing != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<User?> ValidateUserAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !user.IsActive)
            {
                return null;
            }

            var hash = HashPassword(request.Password);
            if (!string.Equals(user.PasswordHash, hash, StringComparison.Ordinal))
            {
                return null;
            }

            return user;
        }

        private static string HashPassword(string password)
        {
            // Đơn giản: SHA256. Sau này có thể đổi sang algo mạnh hơn như BCrypt/Argon2.
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}



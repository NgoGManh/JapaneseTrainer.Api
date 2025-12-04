using JapaneseTrainer.Api.DTOs.Users;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Repositories;

namespace JapaneseTrainer.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllAsync(
            string? email = null,
            string? username = null,
            string? role = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            var query = users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(u => u.Username != null && u.Username.Contains(username));
            }

            if (!string.IsNullOrWhiteSpace(role) &&
                Enum.TryParse<UserRole>(role, true, out var parsedRole))
            {
                query = query.Where(u => u.Role == parsedRole);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            return query
                .OrderByDescending(u => u.CreatedAt)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            return user == null ? null : MapToDto(user);
        }

        public async Task<bool> SetRoleAsync(Guid id, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return false;
            }

            if (!Enum.TryParse<UserRole>(role, true, out var parsedRole))
            {
                return false;
            }

            user.Role = parsedRole;
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return false;
            }

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username ?? string.Empty,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}



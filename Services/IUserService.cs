using JapaneseTrainer.Api.DTOs.Users;

namespace JapaneseTrainer.Api.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync(
            string? email = null,
            string? username = null,
            string? role = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
        Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> SetRoleAsync(Guid id, string role, CancellationToken cancellationToken = default);
        Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    }
}



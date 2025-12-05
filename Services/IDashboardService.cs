using JapaneseTrainer.Api.DTOs.Dashboard;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync(Guid userId, LearningSkill? skill, CancellationToken cancellationToken = default);
    }
}

using JapaneseTrainer.Api.DTOs.AI;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public interface IAIQueueService
    {
        Task<AIJobDto> CreateJobAsync(AIJobRequest request, CancellationToken cancellationToken = default);
        Task<AIJobDto?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<AIJobDto>> GetJobsAsync(AIJobType? type, AIJobStatus? status, CancellationToken cancellationToken = default);
        Task<PagedResult<AIJobDto>> GetJobsPagedAsync(AIJobFilterRequest filter, CancellationToken cancellationToken = default);
        Task<AIJobDto?> ProcessJobAsync(Guid id, CancellationToken cancellationToken = default); // Mock process
    }
}


using JapaneseTrainer.Api.DTOs.Study;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public interface IStudyService
    {
        Task<List<StudyQueueItemDto>> GetQueueAsync(Guid userId, LearningSkill? skill, int limit, CancellationToken cancellationToken = default);
        Task<StudyProgressDto> SubmitAnswerAsync(Guid userId, StudyAnswerRequest request, CancellationToken cancellationToken = default);

        Task<ReviewSessionDto> StartSessionAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ReviewSessionDto?> EndSessionAsync(Guid sessionId, int correctCount, int totalAnswered, CancellationToken cancellationToken = default);
    }
}

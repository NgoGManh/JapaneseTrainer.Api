using JapaneseTrainer.Api.DTOs.Exercises;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public interface IExerciseService
    {
        Task<List<ExerciseDto>> GetExercisesAsync(
            ExerciseType? type = null,
            LearningSkill? skill = null,
            Guid? itemId = null,
            Guid? grammarMasterId = null,
            CancellationToken cancellationToken = default);

        Task<ExerciseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ExerciseDto> CreateAsync(CreateExerciseRequest request, CancellationToken cancellationToken = default);

        Task<ExerciseDto?> UpdateAsync(Guid id, UpdateExerciseRequest request, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}


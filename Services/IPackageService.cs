using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Packages;

namespace JapaneseTrainer.Api.Services
{
    public interface IPackageService
    {
        // Package
        Task<List<PackageDto>> GetPackagesAsync(string? search, Guid? ownerId, bool? isPublic, CancellationToken cancellationToken = default);
        Task<PagedResult<PackageDto>> GetPackagesPagedAsync(PackageFilterRequest filter, CancellationToken cancellationToken = default);
        Task<PackageDto?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PackageDto> CreatePackageAsync(CreatePackageRequest request, CancellationToken cancellationToken = default);
        Task<PackageDto?> UpdatePackageAsync(Guid id, UpdatePackageRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeletePackageAsync(Guid id, CancellationToken cancellationToken = default);

        // Lesson
        Task<List<LessonDto>> GetLessonsAsync(Guid packageId, CancellationToken cancellationToken = default);
        Task<LessonDto?> GetLessonByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<LessonDto> CreateLessonAsync(CreateLessonRequest request, CancellationToken cancellationToken = default);
        Task<LessonDto?> UpdateLessonAsync(Guid id, UpdateLessonRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteLessonAsync(Guid id, CancellationToken cancellationToken = default);

        // Lesson - Item
        Task<LessonDto?> AddLessonItemAsync(Guid lessonId, Guid itemId, CancellationToken cancellationToken = default);
        Task<LessonDto?> RemoveLessonItemAsync(Guid lessonId, Guid itemId, CancellationToken cancellationToken = default);

        // Lesson - Grammar
        Task<LessonDto?> AddLessonGrammarAsync(Guid lessonId, Guid grammarMasterId, Guid? grammarPackageId, CancellationToken cancellationToken = default);
        Task<LessonDto?> RemoveLessonGrammarAsync(Guid lessonId, Guid grammarMasterId, CancellationToken cancellationToken = default);
    }
}

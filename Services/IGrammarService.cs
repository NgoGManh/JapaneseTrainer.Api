using JapaneseTrainer.Api.DTOs.Grammar;

namespace JapaneseTrainer.Api.Services
{
    public interface IGrammarService
    {
        // Grammar Master
        Task<List<GrammarMasterDto>> GetMastersAsync(
            string? search = null,
            string? level = null,
            CancellationToken cancellationToken = default);

        Task<GrammarMasterDto?> GetMasterByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<GrammarMasterDto> CreateMasterAsync(
            CreateGrammarMasterRequest request,
            CancellationToken cancellationToken = default);

        Task<GrammarMasterDto?> UpdateMasterAsync(
            Guid id,
            UpdateGrammarMasterRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteMasterAsync(Guid id, CancellationToken cancellationToken = default);

        // Grammar Package
        Task<List<GrammarPackageDto>> GetPackagesAsync(
            Guid? masterId = null,
            Guid? packageId = null,
            CancellationToken cancellationToken = default);

        Task<GrammarPackageDto?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<GrammarPackageDto> CreatePackageAsync(
            CreateGrammarPackageRequest request,
            CancellationToken cancellationToken = default);

        Task<GrammarPackageDto?> UpdatePackageAsync(
            Guid id,
            UpdateGrammarPackageRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> DeletePackageAsync(Guid id, CancellationToken cancellationToken = default);
    }
}


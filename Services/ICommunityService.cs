using JapaneseTrainer.Api.DTOs.Community;

namespace JapaneseTrainer.Api.Services
{
    public interface ICommunityService
    {
        // Comments
        Task<List<CommentDto>> GetCommentsAsync(Guid? packageId, Guid? userId, CancellationToken cancellationToken = default);
        Task<CommentDto?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentRequest request, CancellationToken cancellationToken = default);
        Task<CommentDto?> UpdateCommentAsync(Guid id, Guid userId, UpdateCommentRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteCommentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        // Reports
        Task<ReportDto> CreateReportAsync(Guid userId, CreateReportRequest request, CancellationToken cancellationToken = default);
        Task<List<ReportDto>> GetReportsAsync(string? status, CancellationToken cancellationToken = default);
        Task<ReportDto?> UpdateReportStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
    }
}


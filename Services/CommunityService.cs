using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Community;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommunityService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Comments

        public async Task<List<CommentDto>> GetCommentsAsync(Guid? packageId, Guid? userId, CancellationToken cancellationToken = default)
        {
            var query = _context.Comments.AsQueryable();

            if (packageId.HasValue)
            {
                query = query.Where(c => c.PackageId == packageId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(c => c.UserId == userId.Value);
            }

            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<CommentDto?> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return comment == null ? null : _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentRequest request, CancellationToken cancellationToken = default)
        {
            // ensure package exists
            var pkgExists = await _context.Packages.AnyAsync(p => p.Id == request.PackageId, cancellationToken);
            if (!pkgExists)
            {
                throw new InvalidOperationException("Package not found");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PackageId = request.PackageId,
                Content = request.Content,
                Rating = request.Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto?> UpdateCommentAsync(Guid id, Guid userId, UpdateCommentRequest request, CancellationToken cancellationToken = default)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (comment == null)
            {
                return null;
            }

            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cannot edit others' comments");
            }

            comment.Content = request.Content;
            comment.Rating = request.Rating;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<bool> DeleteCommentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (comment == null)
            {
                return false;
            }

            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("Cannot delete others' comments");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion

        #region Reports

        public async Task<ReportDto> CreateReportAsync(Guid userId, CreateReportRequest request, CancellationToken cancellationToken = default)
        {
            var report = new Report
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                Reason = request.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reports.AddAsync(report, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ReportDto>(report);
        }

        public async Task<List<ReportDto>> GetReportsAsync(string? status, CancellationToken cancellationToken = default)
        {
            var query = _context.Reports.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ReportDto>>(reports);
        }

        public async Task<ReportDto?> UpdateReportStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            if (report == null)
            {
                return null;
            }

            report.Status = status;
            report.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ReportDto>(report);
        }

        #endregion
    }
}

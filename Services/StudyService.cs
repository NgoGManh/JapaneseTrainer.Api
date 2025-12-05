using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Study;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public class StudyService : IStudyService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private static readonly TimeSpan[] SrsSchedule =
        {
            TimeSpan.FromHours(8),    // stage 0 -> 8h
            TimeSpan.FromDays(1),     // stage 1 -> 1d
            TimeSpan.FromDays(3),     // stage 2 -> 3d
            TimeSpan.FromDays(7),     // stage 3 -> 7d
            TimeSpan.FromDays(21),    // stage 4 -> 21d
            TimeSpan.FromDays(60)     // stage 5 -> 60d
        };

        public StudyService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<StudyQueueItemDto>> GetQueueAsync(Guid userId, LearningSkill? skill, int limit, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var query = _context.StudyProgresses
                .Include(sp => sp.Item)
                .Where(sp => sp.UserId == userId);

            if (skill.HasValue)
            {
                query = query.Where(sp => sp.Skill == skill.Value);
            }

            var due = await query
                .Where(sp => sp.NextReviewAt == null || sp.NextReviewAt <= now)
                .OrderBy(sp => sp.NextReviewAt ?? DateTime.MinValue)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return due.Select(sp => new StudyQueueItemDto
            {
                ItemId = sp.ItemId,
                Japanese = sp.Item.Japanese,
                Meaning = sp.Item.Meaning,
                Skill = sp.Skill,
                Stage = sp.Stage,
                NextReviewAt = sp.NextReviewAt
            }).ToList();
        }

        public async Task<StudyProgressDto> SubmitAnswerAsync(Guid userId, StudyAnswerRequest request, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var progress = await _context.StudyProgresses
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.ItemId == request.ItemId && sp.Skill == request.Skill, cancellationToken);

            if (progress == null)
            {
                progress = new StudyProgress
                {
                    UserId = userId,
                    ItemId = request.ItemId,
                    Skill = request.Skill,
                    Stage = 0,
                    CreatedAt = now
                };
                await _context.StudyProgresses.AddAsync(progress, cancellationToken);
            }

            if (request.IsCorrect)
            {
                progress.Stage = Math.Min(progress.Stage + 1, 5);
                progress.CorrectStreak += 1;
            }
            else
            {
                progress.Stage = Math.Max(progress.Stage - 1, 0);
                progress.CorrectStreak = 0;
                progress.WrongCount += 1;
            }

            progress.LastReviewedAt = now;
            progress.NextReviewAt = now + SrsSchedule[progress.Stage];
            progress.UpdatedAt = now;

            if (request.ReviewSessionId.HasValue)
            {
                var session = await _context.ReviewSessions.FirstOrDefaultAsync(rs => rs.Id == request.ReviewSessionId.Value, cancellationToken);
                if (session != null)
                {
                    session.TotalAnswered += 1;
                    if (request.IsCorrect) session.CorrectCount += 1;
                    session.UpdatedAt = now;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<StudyProgressDto>(progress);
        }

        public async Task<ReviewSessionDto> StartSessionAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var session = new ReviewSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ReviewSessions.AddAsync(session, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ReviewSessionDto>(session);
        }

        public async Task<ReviewSessionDto?> EndSessionAsync(Guid sessionId, int correctCount, int totalAnswered, CancellationToken cancellationToken = default)
        {
            var session = await _context.ReviewSessions.FirstOrDefaultAsync(rs => rs.Id == sessionId, cancellationToken);
            if (session == null)
            {
                return null;
            }

            session.CorrectCount = correctCount;
            session.TotalAnswered = totalAnswered;
            session.EndedAt = DateTime.UtcNow;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ReviewSessionDto>(session);
        }
    }
}

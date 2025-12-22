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
                .Include(sp => sp.Kanji)
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

            return due.Select(sp =>
            {
                if (sp.ItemId.HasValue && sp.Item != null)
                {
                    return new StudyQueueItemDto
                    {
                        ItemId = sp.ItemId,
                        KanjiId = null,
                        Type = "Item",
                        Japanese = sp.Item.Japanese,
                        Meaning = sp.Item.Meaning,
                        AdditionalInfo = sp.Item.Reading,
                        Skill = sp.Skill,
                        Stage = sp.Stage,
                        NextReviewAt = sp.NextReviewAt
                    };
                }
                else if (sp.KanjiId.HasValue && sp.Kanji != null)
                {
                    return new StudyQueueItemDto
                    {
                        ItemId = null,
                        KanjiId = sp.KanjiId,
                        Type = "Kanji",
                        Japanese = sp.Kanji.Character,
                        Meaning = sp.Kanji.MeaningVietnamese ?? sp.Kanji.Meaning,
                        AdditionalInfo = $"音読み: {sp.Kanji.Onyomi ?? ""}, 訓読み: {sp.Kanji.Kunyomi ?? ""}",
                        Skill = sp.Skill,
                        Stage = sp.Stage,
                        NextReviewAt = sp.NextReviewAt
                    };
                }
                return null;
            })
            .Where(dto => dto != null)
            .Cast<StudyQueueItemDto>()
            .ToList();
        }

        public async Task<List<StudyQueueItemDto>> GetQueueByLessonsAsync(Guid userId, GetQueueByLessonsRequest request, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var result = new List<StudyQueueItemDto>();

            // Get lessons with their items and kanjis
            var lessons = await _context.Lessons
                .Include(l => l.LessonItems)
                    .ThenInclude(li => li.Item)
                .Include(l => l.LessonKanjis)
                    .ThenInclude(lk => lk.Kanji)
                .Where(l => request.LessonIds.Contains(l.Id))
                .ToListAsync(cancellationToken);

            // Process Items if requested
            if (request.IncludeItems)
            {
                var itemIds = lessons
                    .SelectMany(l => l.LessonItems)
                    .Select(li => li.ItemId)
                    .Distinct()
                    .ToList();

                if (itemIds.Any())
                {
                    var query = _context.StudyProgresses
                        .Include(sp => sp.Item)
                        .Where(sp => sp.UserId == userId && sp.ItemId.HasValue && itemIds.Contains(sp.ItemId.Value));

                    if (request.Skill.HasValue)
                    {
                        query = query.Where(sp => sp.Skill == request.Skill.Value);
                    }

                    var dueItems = await query
                        .Where(sp => sp.NextReviewAt == null || sp.NextReviewAt <= now)
                        .OrderBy(sp => sp.NextReviewAt ?? DateTime.MinValue)
                        .Take(request.Limit)
                        .ToListAsync(cancellationToken);

                    result.AddRange(dueItems.Where(sp => sp.Item != null).Select(sp => new StudyQueueItemDto
                    {
                        ItemId = sp.ItemId,
                        KanjiId = null,
                        Type = "Item",
                        Japanese = sp.Item!.Japanese,
                        Meaning = sp.Item.Meaning,
                        AdditionalInfo = sp.Item.Reading,
                        Skill = sp.Skill,
                        Stage = sp.Stage,
                        NextReviewAt = sp.NextReviewAt
                    }));
                }
            }

            // Process Kanjis if requested
            if (request.IncludeKanjis)
            {
                var kanjiIds = lessons
                    .SelectMany(l => l.LessonKanjis)
                    .Select(lk => lk.KanjiId)
                    .Distinct()
                    .ToList();

                if (kanjiIds.Any())
                {
                    var query = _context.StudyProgresses
                        .Include(sp => sp.Kanji)
                        .Where(sp => sp.UserId == userId && sp.KanjiId.HasValue && kanjiIds.Contains(sp.KanjiId.Value));

                    if (request.Skill.HasValue)
                    {
                        query = query.Where(sp => sp.Skill == request.Skill.Value);
                    }

                    var dueKanjis = await query
                        .Where(sp => sp.NextReviewAt == null || sp.NextReviewAt <= now)
                        .OrderBy(sp => sp.NextReviewAt ?? DateTime.MinValue)
                        .Take(request.Limit)
                        .ToListAsync(cancellationToken);

                    result.AddRange(dueKanjis.Where(sp => sp.Kanji != null).Select(sp => new StudyQueueItemDto
                    {
                        ItemId = null,
                        KanjiId = sp.KanjiId,
                        Type = "Kanji",
                        Japanese = sp.Kanji!.Character,
                        Meaning = sp.Kanji.MeaningVietnamese ?? sp.Kanji.Meaning,
                        AdditionalInfo = $"音読み: {sp.Kanji.Onyomi ?? ""}, 訓読み: {sp.Kanji.Kunyomi ?? ""}",
                        Skill = sp.Skill,
                        Stage = sp.Stage,
                        NextReviewAt = sp.NextReviewAt
                    }));
                }
            }

            return result.OrderBy(r => r.NextReviewAt ?? DateTime.MinValue)
                .Take(request.Limit)
                .ToList();
        }

        public async Task<List<StudyQueueItemDto>> GetQueueByPackageAsync(Guid userId, GetQueueByPackageRequest request, CancellationToken cancellationToken = default)
        {
            // Get package with lessons
            var package = await _context.Packages
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonItems)
                        .ThenInclude(li => li.Item)
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonKanjis)
                        .ThenInclude(lk => lk.Kanji)
                .FirstOrDefaultAsync(p => p.Id == request.PackageId, cancellationToken);

            if (package == null)
            {
                throw new InvalidOperationException("Package not found");
            }

            // Filter lessons if specific lesson IDs provided
            var lessonsToStudy = request.LessonIds != null && request.LessonIds.Any()
                ? package.Lessons.Where(l => request.LessonIds.Contains(l.Id)).ToList()
                : package.Lessons.ToList();

            if (!lessonsToStudy.Any())
            {
                return new List<StudyQueueItemDto>();
            }

            // Use GetQueueByLessonsAsync logic
            var lessonIds = lessonsToStudy.Select(l => l.Id).ToList();
            var lessonsRequest = new GetQueueByLessonsRequest
            {
                LessonIds = lessonIds,
                Skill = request.Skill,
                Limit = request.Limit,
                IncludeItems = request.IncludeItems,
                IncludeKanjis = request.IncludeKanjis
            };

            return await GetQueueByLessonsAsync(userId, lessonsRequest, cancellationToken);
        }

        public async Task<StudyProgressDto> SubmitAnswerAsync(Guid userId, StudyAnswerRequest request, CancellationToken cancellationToken = default)
        {
            if (!request.ItemId.HasValue && !request.KanjiId.HasValue)
            {
                throw new ArgumentException("Either ItemId or KanjiId must be provided");
            }

            if (request.ItemId.HasValue && request.KanjiId.HasValue)
            {
                throw new ArgumentException("Cannot provide both ItemId and KanjiId");
            }

            var now = DateTime.UtcNow;

            StudyProgress? progress;
            if (request.ItemId.HasValue)
            {
                progress = await _context.StudyProgresses
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.ItemId == request.ItemId.Value && sp.Skill == request.Skill && sp.KanjiId == null, cancellationToken);
            }
            else
            {
                progress = await _context.StudyProgresses
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.KanjiId == request.KanjiId.Value && sp.Skill == request.Skill && sp.ItemId == null, cancellationToken);
            }

            if (progress == null)
            {
                progress = new StudyProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ItemId = request.ItemId,
                    KanjiId = request.KanjiId,
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

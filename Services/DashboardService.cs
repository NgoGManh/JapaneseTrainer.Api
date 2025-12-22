using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Dashboard;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DashboardService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync(Guid userId, LearningSkill? skill, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var dayStart = now.Date;

            // Accuracy: dựa trên ReviewSessions trong ngày hiện tại
            var sessionsToday = await _context.ReviewSessions
                .Where(rs => rs.UserId == userId && rs.StartedAt >= dayStart)
                .ToListAsync(cancellationToken);

            int totalAnswered = sessionsToday.Sum(s => s.TotalAnswered);
            int correct = sessionsToday.Sum(s => s.CorrectCount);
            double accuracy = totalAnswered == 0 ? 0 : (double)correct / totalAnswered * 100.0;

            // SRS due today
            var progressQuery = _context.StudyProgresses
                .Include(sp => sp.Item)
                .Where(sp => sp.UserId == userId);

            if (skill.HasValue)
            {
                progressQuery = progressQuery.Where(sp => sp.Skill == skill.Value);
            }

            var dueToday = await progressQuery
                .Where(sp => sp.NextReviewAt == null || sp.NextReviewAt <= now)
                .OrderBy(sp => sp.NextReviewAt ?? DateTime.MinValue)
                .Take(50)
                .ToListAsync(cancellationToken);

            // Streak: đếm ngày liên tục có phiên ReviewSession (StartedAt) từ hôm nay lùi về
            int streakDays = 0;
            DateTime checkDay = dayStart;
            while (true)
            {
                var hasSession = await _context.ReviewSessions
                    .AnyAsync(rs => rs.UserId == userId && rs.StartedAt >= checkDay && rs.StartedAt < checkDay.AddDays(1), cancellationToken);
                if (hasSession)
                {
                    streakDays += 1;
                    checkDay = checkDay.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            // Difficult items: lấy từ UserDifficultItems, join StudyProgress để biết stage/nextReview
            var difficult = await _context.UserDifficultItems
                .Include(ud => ud.Item)
                .Where(ud => ud.UserId == userId)
                .OrderByDescending(ud => ud.Priority)
                .ThenBy(ud => ud.Item.CreatedAt)
                .Take(20)
                .ToListAsync(cancellationToken);

            // Map outputs - only include Items (not Kanjis) for dashboard
            var srsToday = dueToday
                .Where(sp => sp.ItemId.HasValue && sp.Item != null)
                .Select(sp => new StudyQueueItemShortDto
                {
                    ItemId = sp.ItemId!.Value,
                    Japanese = sp.Item!.Japanese,
                    Meaning = sp.Item.Meaning,
                    Skill = sp.Skill,
                    Stage = sp.Stage,
                    NextReviewAt = sp.NextReviewAt
                }).ToList();

            var difficultDtos = new List<DifficultItemDto>();
            foreach (var d in difficult)
            {
                var sp = await _context.StudyProgresses
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ItemId == d.ItemId && x.Skill == LearningSkill.Read, cancellationToken);
                difficultDtos.Add(new DifficultItemDto
                {
                    ItemId = d.ItemId,
                    Japanese = d.Item.Japanese,
                    Meaning = d.Item.Meaning,
                    Priority = d.Priority,
                    Skill = sp?.Skill ?? LearningSkill.Read,
                    Stage = sp?.Stage ?? 0,
                    NextReviewAt = sp?.NextReviewAt
                });
            }

            return new DashboardOverviewDto
            {
                Accuracy = Math.Round(accuracy, 2),
                ReviewsToday = totalAnswered,
                ReviewsDue = dueToday.Count,
                StreakDays = streakDays,
                SrsToday = srsToday,
                DifficultItems = difficultDtos
            };
        }
    }
}

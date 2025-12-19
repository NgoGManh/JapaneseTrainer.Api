using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Exercises;
using JapaneseTrainer.Api.Helpers;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ExerciseService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ExerciseDto>> GetExercisesAsync(
            ExerciseType? type = null,
            LearningSkill? skill = null,
            Guid? itemId = null,
            Guid? grammarMasterId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Exercises.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(e => e.Type == type.Value);
            }

            if (skill.HasValue)
            {
                query = query.Where(e => e.Skill == skill.Value);
            }

            if (itemId.HasValue)
            {
                query = query.Where(e => e.ItemId == itemId.Value);
            }

            if (grammarMasterId.HasValue)
            {
                query = query.Where(e => e.GrammarMasterId == grammarMasterId.Value);
            }

            var exercises = await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ExerciseDto>>(exercises);
        }

        public async Task<PagedResult<ExerciseDto>> GetExercisesPagedAsync(
            ExerciseFilterRequest filter,
            CancellationToken cancellationToken = default)
        {
            filter.Normalize();
            var query = _context.Exercises.AsQueryable();

            if (filter.Type.HasValue)
            {
                query = query.Where(e => e.Type == filter.Type.Value);
            }

            if (filter.Skill.HasValue)
            {
                query = query.Where(e => e.Skill == filter.Skill.Value);
            }

            if (filter.ItemId.HasValue)
            {
                query = query.Where(e => e.ItemId == filter.ItemId.Value);
            }

            if (filter.GrammarMasterId.HasValue)
            {
                query = query.Where(e => e.GrammarMasterId == filter.GrammarMasterId.Value);
            }

            var sortBy = ConvertSnakeCaseToPascalCase(filter.SortBy ?? "created_at");
            query = query.SortBy(sortBy, filter.OrderBy, "CreatedAt");
            var pagedResult = await query.ToPagedResultAsync(filter.Page, filter.Limit, cancellationToken);

            return new PagedResult<ExerciseDto>(
                _mapper.Map<List<ExerciseDto>>(pagedResult.Items),
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.Limit
            );
        }

        private static string ConvertSnakeCaseToPascalCase(string? snakeCase)
        {
            if (string.IsNullOrWhiteSpace(snakeCase)) return snakeCase ?? string.Empty;
            return string.Join("", snakeCase.Split('_').Select(s => char.ToUpper(s[0]) + s.Substring(1).ToLower()));
        }

        public async Task<ExerciseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            return exercise == null ? null : _mapper.Map<ExerciseDto>(exercise);
        }

        public async Task<ExerciseDto> CreateAsync(CreateExerciseRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureReferencesExist(request.ItemId, request.GrammarMasterId, request.GrammarPackageId, cancellationToken);

            var exercise = new Exercise
            {
                Id = Guid.NewGuid(),
                Type = request.Type,
                Skill = request.Skill,
                Prompt = request.Prompt,
                OptionsJson = request.OptionsJson,
                Answer = request.Answer,
                ItemId = request.ItemId,
                GrammarMasterId = request.GrammarMasterId,
                GrammarPackageId = request.GrammarPackageId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Exercises.AddAsync(exercise, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ExerciseDto>(exercise);
        }

        public async Task<ExerciseDto?> UpdateAsync(Guid id, UpdateExerciseRequest request, CancellationToken cancellationToken = default)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (exercise == null)
            {
                return null;
            }

            await EnsureReferencesExist(request.ItemId, request.GrammarMasterId, request.GrammarPackageId, cancellationToken);

            exercise.Type = request.Type;
            exercise.Skill = request.Skill;
            exercise.Prompt = request.Prompt;
            exercise.OptionsJson = request.OptionsJson;
            exercise.Answer = request.Answer;
            exercise.ItemId = request.ItemId;
            exercise.GrammarMasterId = request.GrammarMasterId;
            exercise.GrammarPackageId = request.GrammarPackageId;
            exercise.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ExerciseDto>(exercise);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (exercise == null)
            {
                return false;
            }

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task EnsureReferencesExist(Guid? itemId, Guid? grammarMasterId, Guid? grammarPackageId, CancellationToken cancellationToken)
        {
            if (itemId.HasValue)
            {
                var exists = await _context.Items.AnyAsync(i => i.Id == itemId.Value, cancellationToken);
                if (!exists)
                {
                    throw new InvalidOperationException("Item not found");
                }
            }

            if (grammarMasterId.HasValue)
            {
                var exists = await _context.GrammarMasters.AnyAsync(g => g.Id == grammarMasterId.Value, cancellationToken);
                if (!exists)
                {
                    throw new InvalidOperationException("Grammar master not found");
                }
            }

            if (grammarPackageId.HasValue)
            {
                var exists = await _context.GrammarPackages.AnyAsync(gp => gp.Id == grammarPackageId.Value, cancellationToken);
                if (!exists)
                {
                    throw new InvalidOperationException("Grammar package not found");
                }
            }
        }
    }
}

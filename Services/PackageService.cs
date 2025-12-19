using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Packages;
using JapaneseTrainer.Api.Helpers;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Services
{
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PackageService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Package

        public async Task<List<PackageDto>> GetPackagesAsync(string? search, Guid? ownerId, bool? isPublic, CancellationToken cancellationToken = default)
        {
            var query = _context.Packages
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonItems)
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonGrammars)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }

            if (ownerId.HasValue)
            {
                query = query.Where(p => p.OwnerId == ownerId.Value);
            }

            if (isPublic.HasValue)
            {
                query = query.Where(p => p.IsPublic == isPublic.Value);
            }

            var packages = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);

            return packages.Select(MapPackageWithLessons).ToList();
        }

        public async Task<PagedResult<PackageDto>> GetPackagesPagedAsync(PackageFilterRequest filter, CancellationToken cancellationToken = default)
        {
            filter.Normalize();
            var query = _context.Packages
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonItems)
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonGrammars)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.Title.Contains(filter.Search) || (p.Description != null && p.Description.Contains(filter.Search)));
            }

            if (filter.OwnerId.HasValue)
            {
                query = query.Where(p => p.OwnerId == filter.OwnerId.Value);
            }

            if (filter.IsPublic.HasValue)
            {
                query = query.Where(p => p.IsPublic == filter.IsPublic.Value);
            }

            query = query.SortBy(filter.SortBy, filter.SortDirection, "CreatedAt");
            var pagedResult = await query.ToPagedResultAsync(filter.PageNumber, filter.PageSize, cancellationToken);

            var dtos = pagedResult.Items.Select(MapPackageWithLessons).ToList();
            return new PagedResult<PackageDto>(dtos, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
        }

        public async Task<PackageDto?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var package = await _context.Packages
                .Include(p => p.Lessons).ThenInclude(l => l.LessonItems)
                .Include(p => p.Lessons).ThenInclude(l => l.LessonGrammars)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            return package == null ? null : MapPackageWithLessons(package);
        }

        public async Task<PackageDto> CreatePackageAsync(CreatePackageRequest request, CancellationToken cancellationToken = default)
        {
            var package = new Package
            {
                Id = Guid.NewGuid(),
                OwnerId = request.OwnerId,
                Title = request.Title,
                Description = request.Description,
                IsPublic = request.IsPublic,
                Level = request.Level,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Packages.AddAsync(package, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapPackageWithLessons(package);
        }

        public async Task<PackageDto?> UpdatePackageAsync(Guid id, UpdatePackageRequest request, CancellationToken cancellationToken = default)
        {
            var package = await _context.Packages
                .Include(p => p.Lessons).ThenInclude(l => l.LessonItems)
                .Include(p => p.Lessons).ThenInclude(l => l.LessonGrammars)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (package == null)
            {
                return null;
            }

            package.Title = request.Title;
            package.Description = request.Description;
            package.IsPublic = request.IsPublic;
            package.Level = request.Level;
            package.Tags = request.Tags;
            package.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return MapPackageWithLessons(package);
        }

        public async Task<bool> DeletePackageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var package = await _context.Packages
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (package == null)
            {
                return false;
            }

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion

        #region Lesson

        public async Task<List<LessonDto>> GetLessonsAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            var lessons = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .Where(l => l.PackageId == packageId)
                .OrderBy(l => l.Order)
                .ThenBy(l => l.CreatedAt)
                .ToListAsync(cancellationToken);

            return lessons.Select(MapLessonDto).ToList();
        }

        public async Task<LessonDto?> GetLessonByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            return lesson == null ? null : MapLessonDto(lesson);
        }

        public async Task<LessonDto> CreateLessonAsync(CreateLessonRequest request, CancellationToken cancellationToken = default)
        {
            // Ensure package exists
            var packageExists = await _context.Packages.AnyAsync(p => p.Id == request.PackageId, cancellationToken);
            if (!packageExists)
            {
                throw new InvalidOperationException("Package not found");
            }

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                PackageId = request.PackageId,
                Title = request.Title,
                Description = request.Description,
                Order = request.Order,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Lessons.AddAsync(lesson, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapLessonDto(lesson);
        }

        public async Task<LessonDto?> UpdateLessonAsync(Guid id, UpdateLessonRequest request, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            lesson.Title = request.Title;
            lesson.Description = request.Description;
            lesson.Order = request.Order;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return MapLessonDto(lesson);
        }

        public async Task<bool> DeleteLessonAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lesson == null)
            {
                return false;
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion

        #region Lesson Items/Grammar

        public async Task<LessonDto?> AddLessonItemAsync(Guid lessonId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            var itemExists = await _context.Items.AnyAsync(i => i.Id == itemId, cancellationToken);
            if (!itemExists)
            {
                throw new InvalidOperationException("Item not found");
            }

            if (!lesson.LessonItems.Any(li => li.ItemId == itemId))
            {
                lesson.LessonItems.Add(new LessonItem { LessonId = lessonId, ItemId = itemId });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return MapLessonDto(lesson);
        }

        public async Task<LessonDto?> RemoveLessonItemAsync(Guid lessonId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            var toRemove = lesson.LessonItems.FirstOrDefault(li => li.ItemId == itemId);
            if (toRemove != null)
            {
                lesson.LessonItems.Remove(toRemove);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return MapLessonDto(lesson);
        }

        public async Task<LessonDto?> AddLessonGrammarAsync(Guid lessonId, Guid grammarMasterId, Guid? grammarPackageId, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            var masterExists = await _context.GrammarMasters.AnyAsync(g => g.Id == grammarMasterId, cancellationToken);
            if (!masterExists)
            {
                throw new InvalidOperationException("Grammar master not found");
            }

            if (grammarPackageId.HasValue)
            {
                var pkgExists = await _context.GrammarPackages.AnyAsync(gp => gp.Id == grammarPackageId.Value, cancellationToken);
                if (!pkgExists)
                {
                    throw new InvalidOperationException("Grammar package not found");
                }
            }

            if (!lesson.LessonGrammars.Any(lg => lg.GrammarMasterId == grammarMasterId))
            {
                lesson.LessonGrammars.Add(new LessonGrammar
                {
                    LessonId = lessonId,
                    GrammarMasterId = grammarMasterId,
                    GrammarPackageId = grammarPackageId
                });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return MapLessonDto(lesson);
        }

        public async Task<LessonDto?> RemoveLessonGrammarAsync(Guid lessonId, Guid grammarMasterId, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            var toRemove = lesson.LessonGrammars.FirstOrDefault(lg => lg.GrammarMasterId == grammarMasterId);
            if (toRemove != null)
            {
                lesson.LessonGrammars.Remove(toRemove);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return MapLessonDto(lesson);
        }

        #endregion

        #region Helpers

        private LessonDto MapLessonDto(Lesson lesson)
        {
            var dto = _mapper.Map<LessonDto>(lesson);
            dto.ItemIds = lesson.LessonItems.Select(li => li.ItemId).ToList();
            dto.GrammarMasterIds = lesson.LessonGrammars.Select(lg => lg.GrammarMasterId).ToList();
            return dto;
        }

        private PackageDto MapPackageWithLessons(Package package)
        {
            var dto = _mapper.Map<PackageDto>(package);
            dto.Lessons = package.Lessons
                .OrderBy(l => l.Order)
                .ThenBy(l => l.CreatedAt)
                .Select(MapLessonDto)
                .ToList();
            return dto;
        }

        #endregion
    }
}

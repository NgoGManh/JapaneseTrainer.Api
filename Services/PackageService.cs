using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Packages;
using JapaneseTrainer.Api.Helpers;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Models.Import;
using MiniExcelLibs;

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
                .Include(p => p.Lessons)
                    .ThenInclude(l => l.LessonKanjis)
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

            var sortBy = ConvertSnakeCaseToPascalCase(filter.SortBy ?? "created_at");
            query = query.SortBy(sortBy, filter.OrderBy, "CreatedAt");
            var pagedResult = await query.ToPagedResultAsync(filter.Page, filter.Limit, cancellationToken);

            var dtos = pagedResult.Items.Select(MapPackageWithLessons).ToList();
            return new PagedResult<PackageDto>(dtos, pagedResult.TotalCount, pagedResult.Page, pagedResult.Limit);
        }

        private static string ConvertSnakeCaseToPascalCase(string? snakeCase)
        {
            if (string.IsNullOrWhiteSpace(snakeCase)) return snakeCase ?? string.Empty;
            return string.Join("", snakeCase.Split('_').Select(s => char.ToUpper(s[0]) + s.Substring(1).ToLower()));
        }

        public async Task<PackageDto?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var package = await _context.Packages
                .Include(p => p.Lessons).ThenInclude(l => l.LessonItems)
                .Include(p => p.Lessons).ThenInclude(l => l.LessonGrammars)
                .Include(p => p.Lessons).ThenInclude(l => l.LessonKanjis)
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
                .Include(p => p.Lessons).ThenInclude(l => l.LessonKanjis)
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
                .Include(l => l.LessonKanjis)
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

            // Add Items if provided
            if (request.ItemIds != null && request.ItemIds.Any())
            {
                var validItemIds = await _context.Items
                    .Where(i => request.ItemIds.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToListAsync(cancellationToken);

                foreach (var itemId in validItemIds)
                {
                    lesson.LessonItems.Add(new LessonItem
                    {
                        LessonId = lesson.Id,
                        ItemId = itemId
                    });
                }
            }

            // Add Grammars if provided
            if (request.GrammarMasterIds != null && request.GrammarMasterIds.Any())
            {
                var validGrammarIds = await _context.GrammarMasters
                    .Where(g => request.GrammarMasterIds.Contains(g.Id))
                    .Select(g => g.Id)
                    .ToListAsync(cancellationToken);

                foreach (var grammarId in validGrammarIds)
                {
                    lesson.LessonGrammars.Add(new LessonGrammar
                    {
                        LessonId = lesson.Id,
                        GrammarMasterId = grammarId
                    });
                }
            }

            // Add Kanjis if provided
            if (request.KanjiIds != null && request.KanjiIds.Any())
            {
                var validKanjiIds = await _context.Kanjis
                    .Where(k => request.KanjiIds.Contains(k.Id))
                    .Select(k => k.Id)
                    .ToListAsync(cancellationToken);

                foreach (var kanjiId in validKanjiIds)
                {
                    lesson.LessonKanjis.Add(new LessonKanji
                    {
                        LessonId = lesson.Id,
                        KanjiId = kanjiId
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Reload with includes
            return MapLessonDto(await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .Include(l => l.LessonKanjis)
                .FirstAsync(l => l.Id == lesson.Id, cancellationToken));
        }

        public async Task<LessonDto?> UpdateLessonAsync(Guid id, UpdateLessonRequest request, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .Include(l => l.LessonKanjis)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            lesson.Title = request.Title;
            lesson.Description = request.Description;
            lesson.Order = request.Order;
            lesson.UpdatedAt = DateTime.UtcNow;

            // Update Items if provided
            if (request.ItemIds != null)
            {
                // Remove all existing items
                _context.LessonItems.RemoveRange(lesson.LessonItems);
                
                // Add new items
                if (request.ItemIds.Any())
                {
                    var validItemIds = await _context.Items
                        .Where(i => request.ItemIds.Contains(i.Id))
                        .Select(i => i.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var itemId in validItemIds)
                    {
                        lesson.LessonItems.Add(new LessonItem
                        {
                            LessonId = lesson.Id,
                            ItemId = itemId
                        });
                    }
                }
            }

            // Update Grammars if provided
            if (request.GrammarMasterIds != null)
            {
                // Remove all existing grammars
                _context.LessonGrammars.RemoveRange(lesson.LessonGrammars);
                
                // Add new grammars
                if (request.GrammarMasterIds.Any())
                {
                    var validGrammarIds = await _context.GrammarMasters
                        .Where(g => request.GrammarMasterIds.Contains(g.Id))
                        .Select(g => g.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var grammarId in validGrammarIds)
                    {
                        lesson.LessonGrammars.Add(new LessonGrammar
                        {
                            LessonId = lesson.Id,
                            GrammarMasterId = grammarId
                        });
                    }
                }
            }

            // Update Kanjis if provided
            if (request.KanjiIds != null)
            {
                // Remove all existing kanjis
                _context.LessonKanjis.RemoveRange(lesson.LessonKanjis);
                
                // Add new kanjis
                if (request.KanjiIds.Any())
                {
                    var validKanjiIds = await _context.Kanjis
                        .Where(k => request.KanjiIds.Contains(k.Id))
                        .Select(k => k.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var kanjiId in validKanjiIds)
                    {
                        lesson.LessonKanjis.Add(new LessonKanji
                        {
                            LessonId = lesson.Id,
                            KanjiId = kanjiId
                        });
                    }
                }
            }

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
                .Include(l => l.LessonKanjis)
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
                .Include(l => l.LessonKanjis)
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
                .Include(l => l.LessonKanjis)
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
                .Include(l => l.LessonKanjis)
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

        public async Task<LessonDto?> AddLessonKanjiAsync(Guid lessonId, Guid kanjiId, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .Include(l => l.LessonKanjis)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            // Check if kanji exists
            var kanjiExists = await _context.Kanjis.AnyAsync(k => k.Id == kanjiId, cancellationToken);
            if (!kanjiExists)
            {
                throw new InvalidOperationException("Kanji not found");
            }

            // Check if already added
            if (!lesson.LessonKanjis.Any(lk => lk.KanjiId == kanjiId))
            {
                lesson.LessonKanjis.Add(new LessonKanji
                {
                    LessonId = lessonId,
                    KanjiId = kanjiId
                });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return MapLessonDto(lesson);
        }

        public async Task<LessonDto?> RemoveLessonKanjiAsync(Guid lessonId, Guid kanjiId, CancellationToken cancellationToken = default)
        {
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .Include(l => l.LessonKanjis)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                return null;
            }

            var toRemove = lesson.LessonKanjis.FirstOrDefault(lk => lk.KanjiId == kanjiId);
            if (toRemove != null)
            {
                lesson.LessonKanjis.Remove(toRemove);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return MapLessonDto(lesson);
        }

        public async Task<LessonImportResultDto> ImportLessonContentAsync(Guid lessonId, Stream excelStream, CancellationToken cancellationToken = default)
        {
            // Check if lesson exists
            var lesson = await _context.Lessons
                .Include(l => l.LessonItems)
                .Include(l => l.LessonGrammars)
                .Include(l => l.LessonKanjis)
                .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

            if (lesson == null)
            {
                throw new InvalidOperationException("Lesson not found");
            }

            // Read Excel data
            var rows = excelStream.Query<LessonContentImportDto>().ToList();

            if (!rows.Any())
            {
                return new LessonImportResultDto
                {
                    Message = "File Excel rỗng hoặc không có dữ liệu hợp lệ",
                    TotalProcessed = 0
                };
            }

            var result = new LessonImportResultDto
            {
                TotalProcessed = rows.Count
            };

            // Process each row
            foreach (var row in rows)
            {
                var type = (row.Type ?? string.Empty).Trim();
                
                if (string.IsNullOrWhiteSpace(type))
                    continue;

                switch (type.ToLower())
                {
                    case "item":
                        await ProcessItemImport(row, lesson, result, cancellationToken);
                        break;
                    case "kanji":
                        await ProcessKanjiImport(row, lesson, result, cancellationToken);
                        break;
                    case "grammar":
                        await ProcessGrammarImport(row, lesson, result, cancellationToken);
                        break;
                }
            }

            // Save all changes
            await _context.SaveChangesAsync(cancellationToken);

            // Build message
            var addedCount = result.ItemsAdded + result.KanjisAdded + result.GrammarsAdded;
            var notFoundCount = result.ItemsNotFound + result.KanjisNotFound + result.GrammarsNotFound;
            var alreadyExistsCount = result.ItemsAlreadyExists + result.KanjisAlreadyExists + result.GrammarsAlreadyExists;
            
            result.Message = $"Đã thêm thành công {addedCount} mục vào bài học! " +
                           $"{notFoundCount} mục không tìm thấy trong database. " +
                           $"{alreadyExistsCount} mục đã tồn tại trong bài học.";

            return result;
        }

        private async Task ProcessItemImport(
            LessonContentImportDto row,
            Lesson lesson,
            LessonImportResultDto result,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(row.Japanese))
            {
                result.ItemsNotFound++;
                result.NotFoundItems.Add("(Japanese trống)");
                return;
            }

            // Find matching item
            var query = _context.Items.Where(i => i.Japanese == row.Japanese.Trim());

            // If Reading is provided, match by both Japanese and Reading
            if (!string.IsNullOrWhiteSpace(row.Reading))
            {
                query = query.Where(i => i.Reading == row.Reading.Trim());
            }

            var item = await query.FirstOrDefaultAsync(cancellationToken);

            if (item == null)
            {
                var notFoundKey = string.IsNullOrWhiteSpace(row.Reading)
                    ? row.Japanese
                    : $"{row.Japanese} ({row.Reading})";
                result.ItemsNotFound++;
                result.NotFoundItems.Add(notFoundKey);
                return;
            }

            // Check if already exists in lesson
            if (lesson.LessonItems.Any(li => li.ItemId == item.Id))
            {
                result.ItemsAlreadyExists++;
                return;
            }

            // Add to lesson
            lesson.LessonItems.Add(new LessonItem
            {
                LessonId = lesson.Id,
                ItemId = item.Id
            });
            result.ItemsAdded++;
        }

        private async Task ProcessKanjiImport(
            LessonContentImportDto row,
            Lesson lesson,
            LessonImportResultDto result,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(row.Character))
            {
                result.KanjisNotFound++;
                result.NotFoundKanjis.Add("(Character trống)");
                return;
            }

            // Find matching kanji
            var kanji = await _context.Kanjis
                .FirstOrDefaultAsync(k => k.Character == row.Character.Trim(), cancellationToken);

            if (kanji == null)
            {
                result.KanjisNotFound++;
                result.NotFoundKanjis.Add(row.Character);
                return;
            }

            // Check if already exists in lesson
            if (lesson.LessonKanjis.Any(lk => lk.KanjiId == kanji.Id))
            {
                result.KanjisAlreadyExists++;
                return;
            }

            // Add to lesson
            lesson.LessonKanjis.Add(new LessonKanji
            {
                LessonId = lesson.Id,
                KanjiId = kanji.Id
            });
            result.KanjisAdded++;
        }

        private async Task ProcessGrammarImport(
            LessonContentImportDto row,
            Lesson lesson,
            LessonImportResultDto result,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(row.Title))
            {
                result.GrammarsNotFound++;
                result.NotFoundGrammars.Add("(Title trống)");
                return;
            }

            // Find matching grammar
            var grammar = await _context.GrammarMasters
                .FirstOrDefaultAsync(g => g.Title == row.Title.Trim(), cancellationToken);

            if (grammar == null)
            {
                result.GrammarsNotFound++;
                result.NotFoundGrammars.Add(row.Title);
                return;
            }

            // Check if already exists in lesson
            if (lesson.LessonGrammars.Any(lg => lg.GrammarMasterId == grammar.Id))
            {
                result.GrammarsAlreadyExists++;
                return;
            }

            // Add to lesson
            lesson.LessonGrammars.Add(new LessonGrammar
            {
                LessonId = lesson.Id,
                GrammarMasterId = grammar.Id
            });
            result.GrammarsAdded++;
        }

        #endregion

        #region Helpers

        private LessonDto MapLessonDto(Lesson lesson)
        {
            var dto = _mapper.Map<LessonDto>(lesson);
            dto.ItemIds = lesson.LessonItems.Select(li => li.ItemId).ToList();
            dto.GrammarMasterIds = lesson.LessonGrammars.Select(lg => lg.GrammarMasterId).ToList();
            dto.KanjiIds = lesson.LessonKanjis.Select(lk => lk.KanjiId).ToList();
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

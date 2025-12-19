using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.Common;
using JapaneseTrainer.Api.DTOs.Grammar;
using JapaneseTrainer.Api.Helpers;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Services
{
    public class GrammarService : IGrammarService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GrammarService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Grammar Master

        public async Task<List<GrammarMasterDto>> GetMastersAsync(
            string? search = null,
            string? level = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.GrammarMasters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(g =>
                    g.Title.Contains(search) ||
                    (g.Meaning != null && g.Meaning.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(g => g.Level == level);
            }

            var masters = await query
                .OrderBy(g => g.Title)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<GrammarMasterDto>>(masters);
        }

        public async Task<PagedResult<GrammarMasterDto>> GetMastersPagedAsync(
            GrammarMasterFilterRequest filter,
            CancellationToken cancellationToken = default)
        {
            filter.Normalize();
            var query = _context.GrammarMasters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(g =>
                    g.Title.Contains(filter.Search) ||
                    (g.Meaning != null && g.Meaning.Contains(filter.Search)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Level))
            {
                query = query.Where(g => g.Level == filter.Level);
            }

            var sortBy = ConvertSnakeCaseToPascalCase(filter.SortBy ?? "created_at");
            query = query.SortBy(sortBy, filter.OrderBy, "CreatedAt");
            var pagedResult = await query.ToPagedResultAsync(filter.Page, filter.Limit, cancellationToken);

            return new PagedResult<GrammarMasterDto>(
                _mapper.Map<List<GrammarMasterDto>>(pagedResult.Items),
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

        public async Task<GrammarMasterDto?> GetMasterByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var master = await _context.GrammarMasters
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            return master == null ? null : _mapper.Map<GrammarMasterDto>(master);
        }

        public async Task<GrammarMasterDto> CreateMasterAsync(
            CreateGrammarMasterRequest request,
            CancellationToken cancellationToken = default)
        {
            var master = new GrammarMaster
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Meaning = request.Meaning,
                Formation = request.Formation,
                Usage = request.Usage,
                Example = request.Example,
                Level = request.Level,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow
            };

            await _context.GrammarMasters.AddAsync(master, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GrammarMasterDto>(master);
        }

        public async Task<GrammarMasterDto?> UpdateMasterAsync(
            Guid id,
            UpdateGrammarMasterRequest request,
            CancellationToken cancellationToken = default)
        {
            var master = await _context.GrammarMasters
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (master == null)
            {
                return null;
            }

            master.Title = request.Title;
            master.Meaning = request.Meaning;
            master.Formation = request.Formation;
            master.Usage = request.Usage;
            master.Example = request.Example;
            master.Level = request.Level;
            master.Tags = request.Tags;
            master.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GrammarMasterDto>(master);
        }

        public async Task<bool> DeleteMasterAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var master = await _context.GrammarMasters
                .Include(g => g.GrammarPackages)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (master == null)
            {
                return false;
            }

            if (master.GrammarPackages.Any())
            {
                throw new InvalidOperationException("Cannot delete grammar master because it is linked to packages.");
            }

            _context.GrammarMasters.Remove(master);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion

        #region Grammar Package

        public async Task<List<GrammarPackageDto>> GetPackagesAsync(
            Guid? masterId = null,
            Guid? packageId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.GrammarPackages
                .Include(gp => gp.GrammarMaster)
                .AsQueryable();

            if (masterId.HasValue)
            {
                query = query.Where(gp => gp.GrammarMasterId == masterId.Value);
            }

            if (packageId.HasValue)
            {
                query = query.Where(gp => gp.PackageId == packageId.Value);
            }

            var packages = await query
                .OrderBy(gp => gp.CustomTitle ?? gp.GrammarMaster.Title)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<GrammarPackageDto>>(packages);
        }

        public async Task<GrammarPackageDto?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var package = await _context.GrammarPackages
                .Include(gp => gp.GrammarMaster)
                .FirstOrDefaultAsync(gp => gp.Id == id, cancellationToken);

            return package == null ? null : _mapper.Map<GrammarPackageDto>(package);
        }

        public async Task<GrammarPackageDto> CreatePackageAsync(
            CreateGrammarPackageRequest request,
            CancellationToken cancellationToken = default)
        {
            // ensure master exists
            var masterExists = await _context.GrammarMasters
                .AnyAsync(g => g.Id == request.GrammarMasterId, cancellationToken);

            if (!masterExists)
            {
                throw new InvalidOperationException("Grammar master not found");
            }

            var package = new GrammarPackage
            {
                Id = Guid.NewGuid(),
                GrammarMasterId = request.GrammarMasterId,
                PackageId = request.PackageId,
                CustomTitle = request.CustomTitle,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _context.GrammarPackages.AddAsync(package, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Need master for DTO
            await _context.Entry(package).Reference(gp => gp.GrammarMaster).LoadAsync(cancellationToken);

            return _mapper.Map<GrammarPackageDto>(package);
        }

        public async Task<GrammarPackageDto?> UpdatePackageAsync(
            Guid id,
            UpdateGrammarPackageRequest request,
            CancellationToken cancellationToken = default)
        {
            var package = await _context.GrammarPackages
                .Include(gp => gp.GrammarMaster)
                .FirstOrDefaultAsync(gp => gp.Id == id, cancellationToken);

            if (package == null)
            {
                return null;
            }

            package.CustomTitle = request.CustomTitle;
            package.Notes = request.Notes;
            package.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<GrammarPackageDto>(package);
        }

        public async Task<bool> DeletePackageAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var package = await _context.GrammarPackages
                .FirstOrDefaultAsync(gp => gp.Id == id, cancellationToken);

            if (package == null)
            {
                return false;
            }

            _context.GrammarPackages.Remove(package);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion
    }
}


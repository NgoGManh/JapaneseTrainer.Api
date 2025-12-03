using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JapaneseTrainer.Api.Data;

namespace JapaneseTrainer.Api.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(AppDbContext dbContext, ILogger<DatabaseInitializer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Dùng migrations chuẩn của EF Core.
                // Nếu database chưa có, MigrateAsync sẽ tạo mới và apply toàn bộ migrations.
                // Nếu đã có, nó sẽ chỉ apply các migration còn thiếu.
                await _dbContext.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrated successfully using EF Core migrations.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when migrating database.");
                throw;
            }
        }
    }
}



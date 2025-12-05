using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();
        public DbSet<Kanji> Kanjis => Set<Kanji>();
        public DbSet<ExampleSentence> ExampleSentences => Set<ExampleSentence>();
        public DbSet<Audio> Audios => Set<Audio>();

        public DbSet<GrammarMaster> GrammarMasters => Set<GrammarMaster>();
        public DbSet<GrammarPackage> GrammarPackages => Set<GrammarPackage>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
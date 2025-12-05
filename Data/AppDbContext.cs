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

        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<LessonItem> LessonItems => Set<LessonItem>();
        public DbSet<LessonGrammar> LessonGrammars => Set<LessonGrammar>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<StudyProgress> StudyProgresses => Set<StudyProgress>();
        public DbSet<ReviewSession> ReviewSessions => Set<ReviewSession>();
        public DbSet<UserDifficultItem> UserDifficultItems => Set<UserDifficultItem>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<AIQueue> AIQueues => Set<AIQueue>();

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Junction tables composite keys
            modelBuilder.Entity<LessonItem>()
                .HasKey(li => new { li.LessonId, li.ItemId });

            modelBuilder.Entity<LessonGrammar>()
                .HasKey(lg => new { lg.LessonId, lg.GrammarMasterId });

            modelBuilder.Entity<StudyProgress>()
                .HasKey(sp => new { sp.UserId, sp.ItemId, sp.Skill });

            modelBuilder.Entity<UserDifficultItem>()
                .HasKey(ud => new { ud.UserId, ud.ItemId });
        }
    }
}
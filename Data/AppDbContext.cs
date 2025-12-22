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
        public DbSet<LessonKanji> LessonKanjis => Set<LessonKanji>();
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

            modelBuilder.Entity<LessonKanji>()
                .HasKey(lk => new { lk.LessonId, lk.KanjiId });

            // StudyProgress: Use Id as primary key, with unique indexes for Item and Kanji
            modelBuilder.Entity<StudyProgress>()
                .HasKey(sp => sp.Id);

            modelBuilder.Entity<StudyProgress>()
                .HasIndex(sp => new { sp.UserId, sp.ItemId, sp.Skill })
                .IsUnique()
                .HasFilter("[ItemId] IS NOT NULL AND [KanjiId] IS NULL")
                .HasDatabaseName("IX_StudyProgress_User_Item_Skill");

            modelBuilder.Entity<StudyProgress>()
                .HasIndex(sp => new { sp.UserId, sp.KanjiId, sp.Skill })
                .IsUnique()
                .HasFilter("[KanjiId] IS NOT NULL AND [ItemId] IS NULL")
                .HasDatabaseName("IX_StudyProgress_User_Kanji_Skill");

            // Add check constraint: Either ItemId or KanjiId must be set, but not both
            modelBuilder.Entity<StudyProgress>()
                .HasCheckConstraint("CK_StudyProgress_ItemOrKanji", 
                    "([ItemId] IS NOT NULL AND [KanjiId] IS NULL) OR ([ItemId] IS NULL AND [KanjiId] IS NOT NULL)");

            modelBuilder.Entity<UserDifficultItem>()
                .HasKey(ud => new { ud.UserId, ud.ItemId });

            // Performance indexes for large tables
            // Items table indexes
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.Type)
                .HasDatabaseName("IX_Items_Type");

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.CreatedAt)
                .HasDatabaseName("IX_Items_CreatedAt");

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.HashKey)
                .IsUnique()
                .HasDatabaseName("IX_Items_HashKey");

            // Composite index for common search patterns
            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.Japanese, i.Reading })
                .HasDatabaseName("IX_Items_Japanese_Reading");

            // DictionaryEntries indexes
            modelBuilder.Entity<DictionaryEntry>()
                .HasIndex(d => d.JlptLevel)
                .HasDatabaseName("IX_DictionaryEntries_JlptLevel");

            modelBuilder.Entity<DictionaryEntry>()
                .HasIndex(d => d.KanjiId)
                .HasDatabaseName("IX_DictionaryEntries_KanjiId");

            modelBuilder.Entity<DictionaryEntry>()
                .HasIndex(d => d.CreatedAt)
                .HasDatabaseName("IX_DictionaryEntries_CreatedAt");

            // Kanji indexes
            modelBuilder.Entity<Kanji>()
                .HasIndex(k => k.Level)
                .HasDatabaseName("IX_Kanjis_Level");

            modelBuilder.Entity<Kanji>()
                .HasIndex(k => k.Character)
                .IsUnique()
                .HasDatabaseName("IX_Kanjis_Character");
        }
    }
}
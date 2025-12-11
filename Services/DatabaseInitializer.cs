using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.Data.Seeds;
using JapaneseTrainer.Api.Helpers;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Models.Import;
using System.IO.Compression;
using System.Text.Json;

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
                await _dbContext.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrated successfully using EF Core migrations.");

                await SeedFromJmDictAsync(cancellationToken);

                await DataSeeder.SeedAsync(_dbContext);
                _logger.LogInformation("Package, Lesson, and Grammar seed completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when migrating database.");
                throw;
            }
        }

        /// <summary>
        /// Seed dictionary data from JMdict JSON file
        /// </summary>
        public async Task SeedFromJmDictAsync(CancellationToken cancellationToken = default)
        {
            if (await _dbContext.Items.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Items already exist in database. Skipping JMdict seed.");
                return;
            }

            var seedsDir = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeds");
            var zipPath = Path.Combine(seedsDir, "jmdict-eng-3.6.1.json.zip");
            if (!File.Exists(zipPath))
            {
                zipPath = Path.Combine(seedsDir, "jmdict-eng-3.6.1.json.zip");
            }

            var jsonPath = Path.Combine(seedsDir, "jmdict-eng-3.6.1.json");

            if (!File.Exists(zipPath) && !File.Exists(jsonPath))
            {
                _logger.LogWarning("JMdict seed file not found. Checked: {ZipPath} and {JsonPath}", zipPath, jsonPath);
                return;
            }

            var useZip = File.Exists(zipPath);
            _logger.LogInformation("Starting JMdict seed from {Path}...", useZip ? zipPath : jsonPath);

            try
            {
                Stream dataStream;
                if (useZip)
                {
                    using var archive = ZipFile.OpenRead(zipPath);
                    var entry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
                    if (entry == null)
                    {
                        _logger.LogWarning("No JSON entry found inside zip {ZipPath}", zipPath);
                        return;
                    }

                    using var zipEntryStream = entry.Open();
                    var memory = new MemoryStream();
                    await zipEntryStream.CopyToAsync(memory, cancellationToken);
                    memory.Position = 0;
                    dataStream = memory;
                }
                else
                {
                    dataStream = File.OpenRead(jsonPath);
                }

                await using var stream = dataStream;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var rootData = await JsonSerializer.DeserializeAsync<JmDictRoot>(stream, options, cancellationToken);
                
                if (rootData?.Words == null || rootData.Words.Count == 0)
                {
                    _logger.LogWarning("No words found in JMdict file.");
                    return;
                }

                _logger.LogInformation("Found {Count} words in JMdict file. Starting import...", rootData.Words.Count);

                var originalAutoDetectChanges = _dbContext.ChangeTracker.AutoDetectChangesEnabled;
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                var itemsToAdd = new List<Item>();
                var batchSize = 200; 
                var processedCount = 0;
                var skippedCount = 0;
                var totalInserted = 0;

                foreach (var entry in rootData.Words)
                {
                    try
                    {
                        var kanjiEntry = entry.Kanji.FirstOrDefault(k => k.Common == true) 
                            ?? entry.Kanji.FirstOrDefault();
                        var kanaEntry = entry.Kana.FirstOrDefault(k => k.Common == true) 
                            ?? entry.Kana.FirstOrDefault();

                        if (kanaEntry == null || string.IsNullOrWhiteSpace(kanaEntry.Text))
                        {
                            skippedCount++;
                            continue;
                        }

                        var primaryText = kanjiEntry?.Text ?? kanaEntry.Text;
                        var reading = kanaEntry.Text;

                        var firstSense = entry.Sense.FirstOrDefault();
                        var englishGloss = firstSense?.Gloss
                            .FirstOrDefault(g => g.Lang == "eng")?.Text 
                            ?? firstSense?.Gloss.FirstOrDefault()?.Text 
                            ?? "";

                        if (string.IsNullOrWhiteSpace(englishGloss))
                        {
                            skippedCount++;
                            continue;
                        }

                        var posRaw = firstSense?.PartOfSpeech.FirstOrDefault() ?? "unknown";

                        var hashKey = ItemHashHelper.GenerateHashKey(primaryText, reading);

                        var exists = await _dbContext.Items
                            .AnyAsync(i => i.HashKey == hashKey, cancellationToken);
                        
                        if (exists)
                        {
                            skippedCount++;
                            continue;
                        }

                        var itemId = Guid.NewGuid();
                        var dictEntryId = Guid.NewGuid();

                        var newItem = new Item
                        {
                            Id = itemId,
                            Japanese = primaryText,
                            Reading = reading,
                            Romaji = null, 
                            Meaning = englishGloss, 
                            Type = "Vocabulary",
                            HashKey = hashKey,
                            CreatedAt = DateTime.UtcNow,
                            DictionaryEntry = new DictionaryEntry
                            {
                                Id = dictEntryId,
                                Japanese = primaryText,
                                Reading = reading,
                                Meaning = englishGloss,
                                PartOfSpeech = posRaw,
                                JlptLevel = null,
                                ItemId = itemId,
                                CreatedAt = DateTime.UtcNow
                            }
                        };

                        itemsToAdd.Add(newItem);
                        processedCount++;

                        if (itemsToAdd.Count >= batchSize)
                        {
                            await _dbContext.Items.AddRangeAsync(itemsToAdd, cancellationToken);
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            
                            _dbContext.ChangeTracker.Clear();
                            
                            totalInserted += itemsToAdd.Count;
                            itemsToAdd.Clear();
                            
                            _logger.LogInformation("✅ Đã nạp {TotalInserted} từ (Processed: {Processed}, Skipped: {Skipped})", 
                                totalInserted, processedCount, skippedCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing entry {Id}: {Message}", entry.Id, ex.Message);
                        skippedCount++;
                    }
                }

                if (itemsToAdd.Any())
                {
                    await _dbContext.Items.AddRangeAsync(itemsToAdd, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _dbContext.ChangeTracker.Clear();
                    totalInserted += itemsToAdd.Count;
                }

                _dbContext.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;

                _logger.LogInformation("🎉 JMdict seed completed! Processed: {Processed}, Skipped: {Skipped}, Total inserted: {Total}", 
                    processedCount, skippedCount, totalInserted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding JMdict data.");
                throw;
            }
        }
    }
}



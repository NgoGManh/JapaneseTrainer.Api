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
                // Dùng migrations chuẩn của EF Core.
                // Nếu database chưa có, MigrateAsync sẽ tạo mới và apply toàn bộ migrations.
                // Nếu đã có, nó sẽ chỉ apply các migration còn thiếu.
                await _dbContext.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrated successfully using EF Core migrations.");

                // Seed JMdict data after migration
                await SeedFromJmDictAsync(cancellationToken);

                // Seed Package, Lesson, and Grammar data
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
            // Kiểm tra nếu DB đã có dữ liệu thì bỏ qua
            if (await _dbContext.Items.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Items already exist in database. Skipping JMdict seed.");
                return;
            }

            var seedsDir = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeds");
            var zipPath = Path.Combine(seedsDir, "jmdict-eng-3.6.1+20251208123023.json.zip");
            if (!File.Exists(zipPath))
            {
                // fallback tên đơn giản hơn nếu file đổi tên
                zipPath = Path.Combine(seedsDir, "jmdict-eng-3.6.1+20251208123023.json.zip");
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

                    // Copy stream to memory so we can keep it after disposing archive
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

                var itemsToAdd = new List<Item>();
                var batchSize = 1000; // Chia nhỏ để insert cho nhanh
                var processedCount = 0;
                var skippedCount = 0;

                foreach (var entry in rootData.Words)
                {
                    try
                    {
                        // Ưu tiên lấy Kanji common hoặc đầu tiên, nếu không có thì lấy Kana common hoặc đầu tiên
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

                        // Lấy nghĩa Tiếng Anh đầu tiên từ sense đầu tiên
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

                        // Map PartOfSpeech - lấy từ sense đầu tiên
                        var posRaw = firstSense?.PartOfSpeech.FirstOrDefault() ?? "unknown";

                        // Tạo HashKey để tránh duplicate (sử dụng helper để đảm bảo tính nhất quán)
                        var hashKey = ItemHashHelper.GenerateHashKey(primaryText, reading);

                        // Kiểm tra duplicate bằng HashKey
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
                            Romaji = null, // JMdict không có, để NULL cho AI điền sau
                            Meaning = englishGloss, // Lưu tạm tiếng Anh
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
                                JlptLevel = null, // Để null, chờ AI phân loại
                                ItemId = itemId,
                                CreatedAt = DateTime.UtcNow
                            }
                        };

                        itemsToAdd.Add(newItem);
                        processedCount++;

                        // Bulk Insert theo batch
                        if (itemsToAdd.Count >= batchSize)
                        {
                            await _dbContext.Items.AddRangeAsync(itemsToAdd, cancellationToken);
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            _logger.LogInformation("Inserted batch: {Processed} items processed, {Skipped} skipped", 
                                processedCount, skippedCount);
                            itemsToAdd.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing entry {Id}: {Message}", entry.Id, ex.Message);
                        skippedCount++;
                    }
                }

                // Save nốt số còn lại
                if (itemsToAdd.Any())
                {
                    await _dbContext.Items.AddRangeAsync(itemsToAdd, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInformation("JMdict seed completed. Processed: {Processed}, Skipped: {Skipped}, Total inserted: {Total}", 
                    processedCount, skippedCount, processedCount - skippedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding JMdict data.");
                throw;
            }
        }
    }
}



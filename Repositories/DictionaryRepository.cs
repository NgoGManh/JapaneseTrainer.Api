using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Repositories
{
    public class DictionaryRepository : IDictionaryRepository
    {
        private readonly AppDbContext _context;

        public DictionaryRepository(AppDbContext context)
        {
            _context = context;
        }

        // Item methods
        public Task<Item?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Items
                .Include(i => i.ExampleSentences)
                .Include(i => i.Audios)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public Task<List<Item>> GetItemsAsync(string? search, string? type, CancellationToken cancellationToken = default)
        {
            var query = GetItemsQuery(search, type);
            return query
                .OrderBy(i => i.Japanese)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<Item> GetItemsQuery(string? search, string? type)
        {
            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i =>
                    i.Japanese.Contains(search) ||
                    (i.Reading != null && i.Reading.Contains(search)) ||
                    i.Meaning.Contains(search) ||
                    (i.MeaningVietnamese != null && i.MeaningVietnamese.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(i => i.Type == type);
            }

            return query;
        }

        public Task<bool> ItemExistsAsync(string hashKey, CancellationToken cancellationToken = default)
        {
            return _context.Items
                .AnyAsync(i => i.HashKey == hashKey, cancellationToken);
        }

        public async Task AddItemAsync(Item item, CancellationToken cancellationToken = default)
        {
            await _context.Items.AddAsync(item, cancellationToken);
        }

        public Task UpdateItemAsync(Item item, CancellationToken cancellationToken = default)
        {
            _context.Items.Update(item);
            return Task.CompletedTask;
        }

        public Task DeleteItemAsync(Item item, CancellationToken cancellationToken = default)
        {
            _context.Items.Remove(item);
            return Task.CompletedTask;
        }

        // DictionaryEntry methods
        public Task<DictionaryEntry?> GetDictionaryEntryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.DictionaryEntries
                .Include(d => d.Kanji)
                .Include(d => d.Item)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public Task<List<DictionaryEntry>> GetDictionaryEntriesAsync(string? search, string? jlptLevel, Guid? kanjiId, CancellationToken cancellationToken = default)
        {
            var query = GetDictionaryEntriesQuery(search, jlptLevel, kanjiId, null);
            return query
                .OrderBy(d => d.Japanese)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<DictionaryEntry> GetDictionaryEntriesQuery(string? search, string? jlptLevel, Guid? kanjiId, string? partOfSpeech)
        {
            var query = _context.DictionaryEntries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.Japanese.Contains(search) ||
                    (d.Reading != null && d.Reading.Contains(search)) ||
                    d.Meaning.Contains(search) ||
                    (d.MeaningVietnamese != null && d.MeaningVietnamese.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(jlptLevel))
            {
                query = query.Where(d => d.JlptLevel == jlptLevel);
            }

            if (kanjiId.HasValue)
            {
                query = query.Where(d => d.KanjiId == kanjiId.Value);
            }

            if (!string.IsNullOrWhiteSpace(partOfSpeech))
            {
                query = query.Where(d => d.PartOfSpeech == partOfSpeech);
            }

            return query;
        }

        public async Task AddDictionaryEntryAsync(DictionaryEntry entry, CancellationToken cancellationToken = default)
        {
            await _context.DictionaryEntries.AddAsync(entry, cancellationToken);
        }

        public Task UpdateDictionaryEntryAsync(DictionaryEntry entry, CancellationToken cancellationToken = default)
        {
            _context.DictionaryEntries.Update(entry);
            return Task.CompletedTask;
        }

        public Task DeleteDictionaryEntryAsync(DictionaryEntry entry, CancellationToken cancellationToken = default)
        {
            _context.DictionaryEntries.Remove(entry);
            return Task.CompletedTask;
        }

        // Kanji methods
        public Task<Kanji?> GetKanjiByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Kanjis
                .Include(k => k.DictionaryEntries)
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
        }

        public Task<Kanji?> GetKanjiByCharacterAsync(string character, CancellationToken cancellationToken = default)
        {
            return _context.Kanjis
                .FirstOrDefaultAsync(k => k.Character == character, cancellationToken);
        }

        public Task<List<Kanji>> GetKanjisAsync(string? search, string? level, CancellationToken cancellationToken = default)
        {
            var query = GetKanjisQuery(search, level, null, null);
            return query
                .OrderBy(k => k.Character)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<Kanji> GetKanjisQuery(string? search, string? level, int? minStrokes, int? maxStrokes)
        {
            var query = _context.Kanjis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(k =>
                    k.Character.Contains(search) ||
                    k.Meaning.Contains(search) ||
                    (k.MeaningVietnamese != null && k.MeaningVietnamese.Contains(search)) ||
                    (k.HanViet != null && k.HanViet.Contains(search)) ||
                    (k.Onyomi != null && k.Onyomi.Contains(search)) ||
                    (k.Kunyomi != null && k.Kunyomi.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(k => k.Level == level);
            }

            if (minStrokes.HasValue)
            {
                query = query.Where(k => k.Strokes >= minStrokes.Value);
            }

            if (maxStrokes.HasValue)
            {
                query = query.Where(k => k.Strokes <= maxStrokes.Value);
            }

            return query;
        }

        public async Task AddKanjiAsync(Kanji kanji, CancellationToken cancellationToken = default)
        {
            await _context.Kanjis.AddAsync(kanji, cancellationToken);
        }

        public Task UpdateKanjiAsync(Kanji kanji, CancellationToken cancellationToken = default)
        {
            _context.Kanjis.Update(kanji);
            return Task.CompletedTask;
        }

        public Task DeleteKanjiAsync(Kanji kanji, CancellationToken cancellationToken = default)
        {
            _context.Kanjis.Remove(kanji);
            return Task.CompletedTask;
        }

        // ExampleSentence methods
        public Task<ExampleSentence?> GetExampleSentenceByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.ExampleSentences
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public Task<List<ExampleSentence>> GetExampleSentencesAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default)
        {
            var query = _context.ExampleSentences.AsQueryable();

            if (itemId.HasValue)
            {
                query = query.Where(e => e.ItemId == itemId.Value);
            }

            if (dictionaryEntryId.HasValue)
            {
                query = query.Where(e => e.DictionaryEntryId == dictionaryEntryId.Value);
            }

            return query
                .OrderBy(e => e.Japanese)
                .ToListAsync(cancellationToken);
        }

        public async Task AddExampleSentenceAsync(ExampleSentence example, CancellationToken cancellationToken = default)
        {
            await _context.ExampleSentences.AddAsync(example, cancellationToken);
        }

        public Task UpdateExampleSentenceAsync(ExampleSentence example, CancellationToken cancellationToken = default)
        {
            _context.ExampleSentences.Update(example);
            return Task.CompletedTask;
        }

        public Task DeleteExampleSentenceAsync(ExampleSentence example, CancellationToken cancellationToken = default)
        {
            _context.ExampleSentences.Remove(example);
            return Task.CompletedTask;
        }

        // Audio methods
        public Task<Audio?> GetAudioByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Audios
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public Task<List<Audio>> GetAudiosAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default)
        {
            var query = _context.Audios.AsQueryable();

            if (itemId.HasValue)
            {
                query = query.Where(a => a.ItemId == itemId.Value);
            }

            if (dictionaryEntryId.HasValue)
            {
                query = query.Where(a => a.DictionaryEntryId == dictionaryEntryId.Value);
            }

            return query
                .OrderBy(a => a.Url)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAudioAsync(Audio audio, CancellationToken cancellationToken = default)
        {
            await _context.Audios.AddAsync(audio, cancellationToken);
        }

        public Task UpdateAudioAsync(Audio audio, CancellationToken cancellationToken = default)
        {
            _context.Audios.Update(audio);
            return Task.CompletedTask;
        }

        public Task DeleteAudioAsync(Audio audio, CancellationToken cancellationToken = default)
        {
            _context.Audios.Remove(audio);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


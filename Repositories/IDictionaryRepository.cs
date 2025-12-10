using JapaneseTrainer.Api.Models;

namespace JapaneseTrainer.Api.Repositories
{
    public interface IDictionaryRepository
    {
        // Item methods
        Task<Item?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Item>> GetItemsAsync(string? search, string? type, CancellationToken cancellationToken = default);
        Task<bool> ItemExistsAsync(string hashKey, CancellationToken cancellationToken = default);
        Task AddItemAsync(Item item, CancellationToken cancellationToken = default);
        Task UpdateItemAsync(Item item, CancellationToken cancellationToken = default);
        Task DeleteItemAsync(Item item, CancellationToken cancellationToken = default);

        // DictionaryEntry methods
        Task<DictionaryEntry?> GetDictionaryEntryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<DictionaryEntry>> GetDictionaryEntriesAsync(string? search, string? jlptLevel, Guid? kanjiId, CancellationToken cancellationToken = default);
        Task AddDictionaryEntryAsync(DictionaryEntry entry, CancellationToken cancellationToken = default);
        Task UpdateDictionaryEntryAsync(DictionaryEntry entry, CancellationToken cancellationToken = default);
        Task DeleteDictionaryEntryAsync(DictionaryEntry entry, CancellationToken cancellationToken = default);

        // Kanji methods
        Task<Kanji?> GetKanjiByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Kanji?> GetKanjiByCharacterAsync(string character, CancellationToken cancellationToken = default);
        Task<List<Kanji>> GetKanjisAsync(string? search, string? level, CancellationToken cancellationToken = default);
        Task AddKanjiAsync(Kanji kanji, CancellationToken cancellationToken = default);
        Task UpdateKanjiAsync(Kanji kanji, CancellationToken cancellationToken = default);
        Task DeleteKanjiAsync(Kanji kanji, CancellationToken cancellationToken = default);

        // ExampleSentence methods
        Task<ExampleSentence?> GetExampleSentenceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ExampleSentence>> GetExampleSentencesAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default);
        Task AddExampleSentenceAsync(ExampleSentence example, CancellationToken cancellationToken = default);
        Task UpdateExampleSentenceAsync(ExampleSentence example, CancellationToken cancellationToken = default);
        Task DeleteExampleSentenceAsync(ExampleSentence example, CancellationToken cancellationToken = default);

        // Audio methods
        Task<Audio?> GetAudioByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Audio>> GetAudiosAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default);
        Task AddAudioAsync(Audio audio, CancellationToken cancellationToken = default);
        Task UpdateAudioAsync(Audio audio, CancellationToken cancellationToken = default);
        Task DeleteAudioAsync(Audio audio, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


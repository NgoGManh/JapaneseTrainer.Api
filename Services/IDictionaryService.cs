using JapaneseTrainer.Api.DTOs.Dictionary;

namespace JapaneseTrainer.Api.Services
{
    public interface IDictionaryService
    {
        // Item methods
        Task<List<ItemDto>> GetItemsAsync(string? search, string? type, CancellationToken cancellationToken = default);
        Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken = default);
        Task<ItemDto?> UpdateItemAsync(Guid id, CreateItemRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);

        // DictionaryEntry methods
        Task<List<DictionaryEntryDto>> GetDictionaryEntriesAsync(string? search, string? jlptLevel, Guid? kanjiId, CancellationToken cancellationToken = default);
        Task<DictionaryEntryDto?> GetDictionaryEntryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DictionaryEntryDto> CreateDictionaryEntryAsync(CreateDictionaryEntryRequest request, CancellationToken cancellationToken = default);
        Task<DictionaryEntryDto?> UpdateDictionaryEntryAsync(Guid id, CreateDictionaryEntryRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteDictionaryEntryAsync(Guid id, CancellationToken cancellationToken = default);

        // Kanji methods
        Task<List<KanjiDto>> GetKanjisAsync(string? search, string? level, CancellationToken cancellationToken = default);
        Task<KanjiDto?> GetKanjiByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<KanjiDto> CreateKanjiAsync(CreateKanjiRequest request, CancellationToken cancellationToken = default);
        Task<KanjiDto?> UpdateKanjiAsync(Guid id, CreateKanjiRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteKanjiAsync(Guid id, CancellationToken cancellationToken = default);

        // ExampleSentence methods
        Task<List<ExampleSentenceDto>> GetExampleSentencesAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default);
        Task<ExampleSentenceDto?> GetExampleSentenceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ExampleSentenceDto> CreateExampleSentenceAsync(CreateExampleSentenceRequest request, CancellationToken cancellationToken = default);
        Task<ExampleSentenceDto?> UpdateExampleSentenceAsync(Guid id, CreateExampleSentenceRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteExampleSentenceAsync(Guid id, CancellationToken cancellationToken = default);

        // Audio methods
        Task<List<AudioDto>> GetAudiosAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default);
        Task<AudioDto?> GetAudioByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AudioDto> CreateAudioAsync(CreateAudioRequest request, CancellationToken cancellationToken = default);
        Task<AudioDto?> UpdateAudioAsync(Guid id, CreateAudioRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAudioAsync(Guid id, CancellationToken cancellationToken = default);
    }
}


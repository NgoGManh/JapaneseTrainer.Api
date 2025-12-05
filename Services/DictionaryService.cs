using AutoMapper;
using JapaneseTrainer.Api.DTOs.Dictionary;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Repositories;

namespace JapaneseTrainer.Api.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly IDictionaryRepository _repository;
        private readonly IMapper _mapper;

        public DictionaryService(IDictionaryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Item methods
        public async Task<List<ItemDto>> GetItemsAsync(string? search, string? type, CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetItemsAsync(search, type, cancellationToken);
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _repository.GetItemByIdAsync(id, cancellationToken);
            return item == null ? null : _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken = default)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Japanese = request.Japanese,
                Reading = request.Reading,
                Romaji = request.Romaji,
                Meaning = request.Meaning,
                Type = request.Type,
                HashKey = request.HashKey,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddItemAsync(item, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto?> UpdateItemAsync(Guid id, CreateItemRequest request, CancellationToken cancellationToken = default)
        {
            var item = await _repository.GetItemByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return null;
            }

            item.Japanese = request.Japanese;
            item.Reading = request.Reading;
            item.Romaji = request.Romaji;
            item.Meaning = request.Meaning;
            item.Type = request.Type;
            item.HashKey = request.HashKey;
            item.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateItemAsync(item, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _repository.GetItemByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return false;
            }

            await _repository.DeleteItemAsync(item, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        // DictionaryEntry methods
        public async Task<List<DictionaryEntryDto>> GetDictionaryEntriesAsync(string? search, string? jlptLevel, Guid? kanjiId, CancellationToken cancellationToken = default)
        {
            var entries = await _repository.GetDictionaryEntriesAsync(search, jlptLevel, kanjiId, cancellationToken);
            return _mapper.Map<List<DictionaryEntryDto>>(entries);
        }

        public async Task<DictionaryEntryDto?> GetDictionaryEntryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entry = await _repository.GetDictionaryEntryByIdAsync(id, cancellationToken);
            return entry == null ? null : _mapper.Map<DictionaryEntryDto>(entry);
        }

        public async Task<DictionaryEntryDto> CreateDictionaryEntryAsync(CreateDictionaryEntryRequest request, CancellationToken cancellationToken = default)
        {
            var entry = new DictionaryEntry
            {
                Id = Guid.NewGuid(),
                Japanese = request.Japanese,
                Reading = request.Reading,
                Romaji = request.Romaji,
                Meaning = request.Meaning,
                PartOfSpeech = request.PartOfSpeech,
                JlptLevel = request.JlptLevel,
                KanjiId = request.KanjiId,
                ItemId = request.ItemId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddDictionaryEntryAsync(entry, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DictionaryEntryDto>(entry);
        }

        public async Task<DictionaryEntryDto?> UpdateDictionaryEntryAsync(Guid id, CreateDictionaryEntryRequest request, CancellationToken cancellationToken = default)
        {
            var entry = await _repository.GetDictionaryEntryByIdAsync(id, cancellationToken);
            if (entry == null)
            {
                return null;
            }

            entry.Japanese = request.Japanese;
            entry.Reading = request.Reading;
            entry.Romaji = request.Romaji;
            entry.Meaning = request.Meaning;
            entry.PartOfSpeech = request.PartOfSpeech;
            entry.JlptLevel = request.JlptLevel;
            entry.KanjiId = request.KanjiId;
            entry.ItemId = request.ItemId;
            entry.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateDictionaryEntryAsync(entry, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DictionaryEntryDto>(entry);
        }

        public async Task<bool> DeleteDictionaryEntryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entry = await _repository.GetDictionaryEntryByIdAsync(id, cancellationToken);
            if (entry == null)
            {
                return false;
            }

            await _repository.DeleteDictionaryEntryAsync(entry, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Kanji methods
        public async Task<List<KanjiDto>> GetKanjisAsync(string? search, string? level, CancellationToken cancellationToken = default)
        {
            var kanjis = await _repository.GetKanjisAsync(search, level, cancellationToken);
            return _mapper.Map<List<KanjiDto>>(kanjis);
        }

        public async Task<KanjiDto?> GetKanjiByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var kanji = await _repository.GetKanjiByIdAsync(id, cancellationToken);
            return kanji == null ? null : _mapper.Map<KanjiDto>(kanji);
        }

        public async Task<KanjiDto> CreateKanjiAsync(CreateKanjiRequest request, CancellationToken cancellationToken = default)
        {
            var kanji = new Kanji
            {
                Id = Guid.NewGuid(),
                Character = request.Character,
                Meaning = request.Meaning,
                Onyomi = request.Onyomi,
                Kunyomi = request.Kunyomi,
                Strokes = request.Strokes,
                Level = request.Level,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddKanjiAsync(kanji, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<KanjiDto>(kanji);
        }

        public async Task<KanjiDto?> UpdateKanjiAsync(Guid id, CreateKanjiRequest request, CancellationToken cancellationToken = default)
        {
            var kanji = await _repository.GetKanjiByIdAsync(id, cancellationToken);
            if (kanji == null)
            {
                return null;
            }

            kanji.Character = request.Character;
            kanji.Meaning = request.Meaning;
            kanji.Onyomi = request.Onyomi;
            kanji.Kunyomi = request.Kunyomi;
            kanji.Strokes = request.Strokes;
            kanji.Level = request.Level;
            kanji.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateKanjiAsync(kanji, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<KanjiDto>(kanji);
        }

        public async Task<bool> DeleteKanjiAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var kanji = await _repository.GetKanjiByIdAsync(id, cancellationToken);
            if (kanji == null)
            {
                return false;
            }

            await _repository.DeleteKanjiAsync(kanji, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        // ExampleSentence methods
        public async Task<List<ExampleSentenceDto>> GetExampleSentencesAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default)
        {
            var examples = await _repository.GetExampleSentencesAsync(itemId, dictionaryEntryId, cancellationToken);
            return _mapper.Map<List<ExampleSentenceDto>>(examples);
        }

        public async Task<ExampleSentenceDto?> GetExampleSentenceByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var example = await _repository.GetExampleSentenceByIdAsync(id, cancellationToken);
            return example == null ? null : _mapper.Map<ExampleSentenceDto>(example);
        }

        public async Task<ExampleSentenceDto> CreateExampleSentenceAsync(CreateExampleSentenceRequest request, CancellationToken cancellationToken = default)
        {
            var example = new ExampleSentence
            {
                Id = Guid.NewGuid(),
                Japanese = request.Japanese,
                Reading = request.Reading,
                Romaji = request.Romaji,
                Meaning = request.Meaning,
                ItemId = request.ItemId,
                DictionaryEntryId = request.DictionaryEntryId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddExampleSentenceAsync(example, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ExampleSentenceDto>(example);
        }

        public async Task<ExampleSentenceDto?> UpdateExampleSentenceAsync(Guid id, CreateExampleSentenceRequest request, CancellationToken cancellationToken = default)
        {
            var example = await _repository.GetExampleSentenceByIdAsync(id, cancellationToken);
            if (example == null)
            {
                return null;
            }

            example.Japanese = request.Japanese;
            example.Reading = request.Reading;
            example.Romaji = request.Romaji;
            example.Meaning = request.Meaning;
            example.ItemId = request.ItemId;
            example.DictionaryEntryId = request.DictionaryEntryId;
            example.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateExampleSentenceAsync(example, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ExampleSentenceDto>(example);
        }

        public async Task<bool> DeleteExampleSentenceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var example = await _repository.GetExampleSentenceByIdAsync(id, cancellationToken);
            if (example == null)
            {
                return false;
            }

            await _repository.DeleteExampleSentenceAsync(example, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Audio methods
        public async Task<List<AudioDto>> GetAudiosAsync(Guid? itemId, Guid? dictionaryEntryId, CancellationToken cancellationToken = default)
        {
            var audios = await _repository.GetAudiosAsync(itemId, dictionaryEntryId, cancellationToken);
            return _mapper.Map<List<AudioDto>>(audios);
        }

        public async Task<AudioDto?> GetAudioByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var audio = await _repository.GetAudioByIdAsync(id, cancellationToken);
            return audio == null ? null : _mapper.Map<AudioDto>(audio);
        }

        public async Task<AudioDto> CreateAudioAsync(CreateAudioRequest request, CancellationToken cancellationToken = default)
        {
            var audio = new Audio
            {
                Id = Guid.NewGuid(),
                Url = request.Url,
                Type = request.Type,
                ItemId = request.ItemId,
                DictionaryEntryId = request.DictionaryEntryId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAudioAsync(audio, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AudioDto>(audio);
        }

        public async Task<AudioDto?> UpdateAudioAsync(Guid id, CreateAudioRequest request, CancellationToken cancellationToken = default)
        {
            var audio = await _repository.GetAudioByIdAsync(id, cancellationToken);
            if (audio == null)
            {
                return null;
            }

            audio.Url = request.Url;
            audio.Type = request.Type;
            audio.ItemId = request.ItemId;
            audio.DictionaryEntryId = request.DictionaryEntryId;
            audio.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAudioAsync(audio, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AudioDto>(audio);
        }

        public async Task<bool> DeleteAudioAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var audio = await _repository.GetAudioByIdAsync(id, cancellationToken);
            if (audio == null)
            {
                return false;
            }

            await _repository.DeleteAudioAsync(audio, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}


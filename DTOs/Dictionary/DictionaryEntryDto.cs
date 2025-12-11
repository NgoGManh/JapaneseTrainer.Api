namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class DictionaryEntryDto
    {
        public Guid Id { get; set; }
        public string Japanese { get; set; } = string.Empty;
        public string? Reading { get; set; }
        public string? Romaji { get; set; }
        public string Meaning { get; set; } = string.Empty;
        public string? MeaningVietnamese { get; set; }
        public string DisplayMeaning { get; set; } = string.Empty;
        public string? PartOfSpeech { get; set; }
        public string? JlptLevel { get; set; }
        public Guid? KanjiId { get; set; }
        public Guid? ItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


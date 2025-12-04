namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class ExampleSentenceDto
    {
        public Guid Id { get; set; }
        public string Japanese { get; set; } = string.Empty;
        public string? Reading { get; set; }
        public string? Romaji { get; set; }
        public string Meaning { get; set; } = string.Empty;
        public Guid? ItemId { get; set; }
        public Guid? DictionaryEntryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


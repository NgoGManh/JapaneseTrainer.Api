namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class AudioDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Type { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? DictionaryEntryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


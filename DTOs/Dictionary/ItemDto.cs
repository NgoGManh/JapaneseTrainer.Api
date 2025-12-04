namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Japanese { get; set; } = string.Empty;
        public string? Reading { get; set; }
        public string? Romaji { get; set; }
        public string Meaning { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? HashKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class KanjiDto
    {
        public Guid Id { get; set; }
        public string Character { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public string? Onyomi { get; set; }
        public string? Kunyomi { get; set; }
        public int? Strokes { get; set; }
        public string? Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


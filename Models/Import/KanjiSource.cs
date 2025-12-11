using System.Text.Json.Serialization;

namespace JapaneseTrainer.Api.Models.Import
{
    public class KanjiRoot
    {
        [JsonPropertyName("characters")]
        public List<KanjiSource> Characters { get; set; } = new();
    }

    public class KanjiSource
    {
        [JsonPropertyName("literal")]
        public string Literal { get; set; } = string.Empty;

        [JsonPropertyName("readingMeaning")]
        public KanjiReadingMeaning ReadingMeaning { get; set; } = new();

        [JsonPropertyName("misc")]
        public KanjiMisc Misc { get; set; } = new();
    }

    public class KanjiReadingMeaning
    {
        [JsonPropertyName("groups")]
        public List<KanjiGroup> Groups { get; set; } = new();

        [JsonPropertyName("nanori")]
        public List<string> Nanori { get; set; } = new();
    }

    public class KanjiGroup
    {
        [JsonPropertyName("readings")]
        public List<KanjiReading> Readings { get; set; } = new();

        [JsonPropertyName("meanings")]
        public List<KanjiMeaning> Meanings { get; set; } = new();
    }

    public class KanjiReading
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class KanjiMeaning
    {
        [JsonPropertyName("lang")]
        public string? Lang { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class KanjiMisc
    {
        [JsonPropertyName("strokeCounts")]
        public List<int>? StrokeCounts { get; set; }

        [JsonPropertyName("jlptLevel")]
        public int? JlptLevel { get; set; }

        [JsonPropertyName("grade")]
        public int? Grade { get; set; }
    }
}


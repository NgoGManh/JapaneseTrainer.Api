using System.Text.Json.Serialization;

namespace JapaneseTrainer.Api.Models.Import
{
    /// <summary>
    /// Root structure of JMdict JSON file
    /// </summary>
    public class JmDictRoot
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("words")]
        public List<JmDictSource> Words { get; set; } = new();
    }

    /// <summary>
    /// Single dictionary entry from JMdict
    /// </summary>
    public class JmDictSource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("kanji")]
        public List<JmKanji> Kanji { get; set; } = new();

        [JsonPropertyName("kana")]
        public List<JmKana> Kana { get; set; } = new();

        [JsonPropertyName("sense")]
        public List<JmSense> Sense { get; set; } = new();
    }

    public class JmKanji
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("common")]
        public bool? Common { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();
    }

    public class JmKana
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("common")]
        public bool? Common { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("appliesToKanji")]
        public List<string> AppliesToKanji { get; set; } = new();
    }

    public class JmSense
    {
        [JsonPropertyName("partOfSpeech")]
        public List<string> PartOfSpeech { get; set; } = new();

        [JsonPropertyName("gloss")]
        public List<JmGloss> Gloss { get; set; } = new();

        [JsonPropertyName("appliesToKanji")]
        public List<string> AppliesToKanji { get; set; } = new();

        [JsonPropertyName("appliesToKana")]
        public List<string> AppliesToKana { get; set; } = new();
    }

    public class JmGloss
    {
        [JsonPropertyName("lang")]
        public string Lang { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
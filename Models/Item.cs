using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Core learning unit - represents a vocabulary word, phrase, or grammar structure
    /// </summary>
    public class Item : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Japanese { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Reading { get; set; }

        [MaxLength(200)]
        public string? Romaji { get; set; }

        [Required]
        [MaxLength(500)]
        public string Meaning { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Type { get; set; } // Vocabulary, Phrase, Sentence, etc.

        [MaxLength(100)]
        public string? HashKey { get; set; } // For deduplication

        // Navigation properties
        public virtual ICollection<ExampleSentence> ExampleSentences { get; set; } = new List<ExampleSentence>();
        public virtual ICollection<Audio> Audios { get; set; } = new List<Audio>();
        // TODO: Uncomment when LessonItem model is created in Feature E
        // public virtual ICollection<LessonItem> LessonItems { get; set; } = new List<LessonItem>();
        public virtual DictionaryEntry? DictionaryEntry { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Dictionary entry - detailed dictionary information for a word
    /// </summary>
    public class DictionaryEntry : AuditableEntity
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

        [MaxLength(500)]
        public string? MeaningVietnamese { get; set; }

        [MaxLength(50)]
        public string? PartOfSpeech { get; set; } // Noun, Verb, Adjective, etc.

        [MaxLength(10)]
        public string? JlptLevel { get; set; } // N5, N4, N3, N2, N1

        public Guid? KanjiId { get; set; }

        public Guid? ItemId { get; set; } // Link to Item if exists

        // Navigation properties
        [ForeignKey(nameof(KanjiId))]
        public virtual Kanji? Kanji { get; set; }

        [ForeignKey(nameof(ItemId))]
        public virtual Item? Item { get; set; }
    }
}


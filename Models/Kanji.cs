using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Kanji character with readings and meanings
    /// </summary>
    public class Kanji : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Character { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Meaning { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? MeaningVietnamese { get; set; }

        [MaxLength(50)]
        public string? HanViet { get; set; }

        [MaxLength(200)]
        public string? Onyomi { get; set; } // Chinese reading

        [MaxLength(200)]
        public string? Kunyomi { get; set; } // Native reading

        public int? Strokes { get; set; }

        [MaxLength(10)]
        public string? Level { get; set; } // Grade level or JLPT level

        // Navigation properties
        public virtual ICollection<DictionaryEntry> DictionaryEntries { get; set; } = new List<DictionaryEntry>();
    }
}


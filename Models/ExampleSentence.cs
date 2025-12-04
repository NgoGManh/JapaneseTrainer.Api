using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Example sentence for vocabulary or grammar
    /// </summary>
    public class ExampleSentence : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Japanese { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Reading { get; set; }

        [MaxLength(500)]
        public string? Romaji { get; set; }

        [Required]
        [MaxLength(500)]
        public string Meaning { get; set; } = string.Empty;

        public Guid? ItemId { get; set; }

        public Guid? DictionaryEntryId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ItemId))]
        public virtual Item? Item { get; set; }

        [ForeignKey(nameof(DictionaryEntryId))]
        public virtual DictionaryEntry? DictionaryEntry { get; set; }
    }
}


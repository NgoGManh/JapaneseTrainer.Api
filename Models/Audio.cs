using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Audio file for vocabulary or grammar
    /// </summary>
    public class Audio : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Type { get; set; } // Pronunciation, Example, etc.

        public Guid? ItemId { get; set; }

        public Guid? DictionaryEntryId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ItemId))]
        public virtual Item? Item { get; set; }

        [ForeignKey(nameof(DictionaryEntryId))]
        public virtual DictionaryEntry? DictionaryEntry { get; set; }
    }
}


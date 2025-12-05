using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Đánh dấu item khó cho user
    /// </summary>
    public class UserDifficultItem : AuditableEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        public int Priority { get; set; } = 0; // 0-10 tuỳ ý

        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;
    }
}


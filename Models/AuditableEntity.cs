using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Base model đơn giản cho auditing: thời gian và user tạo/cập nhật.
    /// Có thể kế thừa trong các entity khác (User, Item, Package,...).
    /// </summary>
    public abstract class AuditableEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }
    }
}



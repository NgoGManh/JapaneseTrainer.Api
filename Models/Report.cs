using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    public class Report : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TargetType { get; set; } = string.Empty; // e.g., Package, Comment, Item, etc.

        [Required]
        public Guid TargetId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending/Resolved/Rejected
    }
}


using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    public class Comment : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid PackageId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public int Rating { get; set; } = 0; // optional rating 0-5
    }
}


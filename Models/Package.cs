using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Gói học (từ vựng/ngữ pháp hoặc mixed)
    /// </summary>
    public class Package : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid? OwnerId { get; set; } // optional: user tạo gói

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsPublic { get; set; } = true;

        [MaxLength(20)]
        public string? Level { get; set; }

        [MaxLength(200)]
        public string? Tags { get; set; } // CSV tags

        // Navigation
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}


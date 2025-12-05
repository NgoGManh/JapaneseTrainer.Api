using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Bài học thuộc một package
    /// </summary>
    public class Lesson : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PackageId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public int Order { get; set; } = 0;

        // Navigation
        [ForeignKey(nameof(PackageId))]
        public Package Package { get; set; } = null!;

        public ICollection<LessonItem> LessonItems { get; set; } = new List<LessonItem>();
        public ICollection<LessonGrammar> LessonGrammars { get; set; } = new List<LessonGrammar>();
    }
}


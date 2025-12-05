using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Bài tập: liên kết tới Item hoặc Grammar, chứa dữ liệu JSON cho options/answers
    /// </summary>
    public class Exercise : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public ExerciseType Type { get; set; }

        [Required]
        public LearningSkill Skill { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Prompt { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? OptionsJson { get; set; } // tuỳ kiểu bài: danh sách đáp án/option

        [MaxLength(2000)]
        public string? Answer { get; set; } // đáp án đúng, format tuỳ Type

        public Guid? ItemId { get; set; }
        public Guid? GrammarMasterId { get; set; }
        public Guid? GrammarPackageId { get; set; }

        // Navigation
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(GrammarMasterId))]
        public GrammarMaster? GrammarMaster { get; set; }

        [ForeignKey(nameof(GrammarPackageId))]
        public GrammarPackage? GrammarPackage { get; set; }
    }
}


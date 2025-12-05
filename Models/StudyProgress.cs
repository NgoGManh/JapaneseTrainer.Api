using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Trạng thái SRS của một Item theo Skill cho từng User
    /// </summary>
    public class StudyProgress : AuditableEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public LearningSkill Skill { get; set; }

        public int Stage { get; set; } = 0; // 0-5

        public DateTime? LastReviewedAt { get; set; }
        public DateTime? NextReviewAt { get; set; }

        public int CorrectStreak { get; set; } = 0;
        public int WrongCount { get; set; } = 0;

        // Navigation
        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;
    }
}


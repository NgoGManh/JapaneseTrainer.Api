using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.Study
{
    public class StudyQueueItemDto
    {
        /// <summary>
        /// Item ID (for vocabulary). Null if this is a Kanji.
        /// </summary>
        public Guid? ItemId { get; set; }

        /// <summary>
        /// Kanji ID (for kanji study). Null if this is an Item.
        /// </summary>
        public Guid? KanjiId { get; set; }

        /// <summary>
        /// Type of study item: "Item" or "Kanji"
        /// </summary>
        public string Type { get; set; } = "Item";

        /// <summary>
        /// Japanese text (for Item) or Character (for Kanji)
        /// </summary>
        public string Japanese { get; set; } = string.Empty;

        /// <summary>
        /// Meaning text
        /// </summary>
        public string Meaning { get; set; } = string.Empty;

        /// <summary>
        /// Additional info: Reading for Item, Onyomi/Kunyomi for Kanji
        /// </summary>
        public string? AdditionalInfo { get; set; }

        public LearningSkill Skill { get; set; }
        public int Stage { get; set; }
        public DateTime? NextReviewAt { get; set; }
    }
}

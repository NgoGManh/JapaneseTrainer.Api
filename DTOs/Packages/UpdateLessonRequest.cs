using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class UpdateLessonRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public int Order { get; set; } = 0;

        /// <summary>
        /// List of Item IDs to include in this lesson (vocabulary)
        /// </summary>
        public List<Guid>? ItemIds { get; set; }

        /// <summary>
        /// List of Grammar Master IDs to include in this lesson
        /// </summary>
        public List<Guid>? GrammarMasterIds { get; set; }

        /// <summary>
        /// List of Kanji IDs to include in this lesson
        /// </summary>
        public List<Guid>? KanjiIds { get; set; }
    }
}

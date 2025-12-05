using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Liên kết N-N giữa Lesson và Item
    /// </summary>
    public class LessonItem
    {
        public Guid LessonId { get; set; }
        public Guid ItemId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;
    }
}


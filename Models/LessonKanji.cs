using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Liên kết N-N giữa Lesson và Kanji
    /// </summary>
    public class LessonKanji
    {
        public Guid LessonId { get; set; }
        public Guid KanjiId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(KanjiId))]
        public Kanji Kanji { get; set; } = null!;
    }
}


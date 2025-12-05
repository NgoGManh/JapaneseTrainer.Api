using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Liên kết N-N giữa Lesson và Grammar (master/package)
    /// </summary>
    public class LessonGrammar
    {
        public Guid LessonId { get; set; }
        public Guid GrammarMasterId { get; set; }
        public Guid? GrammarPackageId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(GrammarMasterId))]
        public GrammarMaster GrammarMaster { get; set; } = null!;

        [ForeignKey(nameof(GrammarPackageId))]
        public GrammarPackage? GrammarPackage { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class AddLessonGrammarRequest
    {
        [Required]
        public Guid GrammarMasterId { get; set; }

        public Guid? GrammarPackageId { get; set; }
    }
}

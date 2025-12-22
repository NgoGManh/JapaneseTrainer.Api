using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class AddLessonKanjiRequest
    {
        [Required]
        public Guid KanjiId { get; set; }
    }
}


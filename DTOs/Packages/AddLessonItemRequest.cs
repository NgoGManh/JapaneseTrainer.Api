using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class AddLessonItemRequest
    {
        [Required]
        public Guid ItemId { get; set; }
    }
}

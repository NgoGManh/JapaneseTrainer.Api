using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class CreateAudioRequest
    {
        [Required(ErrorMessage = "Url is required")]
        [MaxLength(500, ErrorMessage = "Url must not exceed 500 characters")]
        [Url(ErrorMessage = "Url must be a valid URL")]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Type must not exceed 50 characters")]
        public string? Type { get; set; }

        public Guid? ItemId { get; set; }

        public Guid? DictionaryEntryId { get; set; }
    }
}


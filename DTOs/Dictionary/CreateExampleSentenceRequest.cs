using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class CreateExampleSentenceRequest
    {
        [Required(ErrorMessage = "Japanese text is required")]
        [MaxLength(500, ErrorMessage = "Japanese text must not exceed 500 characters")]
        public string Japanese { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Reading must not exceed 500 characters")]
        public string? Reading { get; set; }

        [MaxLength(500, ErrorMessage = "Romaji must not exceed 500 characters")]
        public string? Romaji { get; set; }

        [Required(ErrorMessage = "Meaning is required")]
        [MaxLength(500, ErrorMessage = "Meaning must not exceed 500 characters")]
        public string Meaning { get; set; } = string.Empty;

        public Guid? ItemId { get; set; }

        public Guid? DictionaryEntryId { get; set; }
    }
}


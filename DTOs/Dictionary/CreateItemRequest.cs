using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class CreateItemRequest
    {
        [Required(ErrorMessage = "Japanese text is required")]
        [MaxLength(200, ErrorMessage = "Japanese text must not exceed 200 characters")]
        public string Japanese { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Reading must not exceed 200 characters")]
        public string? Reading { get; set; }

        [MaxLength(200, ErrorMessage = "Romaji must not exceed 200 characters")]
        public string? Romaji { get; set; }

        [Required(ErrorMessage = "Meaning is required")]
        [MaxLength(500, ErrorMessage = "Meaning must not exceed 500 characters")]
        public string Meaning { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Type must not exceed 50 characters")]
        public string? Type { get; set; }

        [MaxLength(100, ErrorMessage = "HashKey must not exceed 100 characters")]
        public string? HashKey { get; set; }
    }
}


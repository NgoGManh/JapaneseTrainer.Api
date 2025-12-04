using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Dictionary
{
    public class CreateKanjiRequest
    {
        [Required(ErrorMessage = "Character is required")]
        [MaxLength(10, ErrorMessage = "Character must not exceed 10 characters")]
        public string Character { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meaning is required")]
        [MaxLength(200, ErrorMessage = "Meaning must not exceed 200 characters")]
        public string Meaning { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Onyomi must not exceed 200 characters")]
        public string? Onyomi { get; set; }

        [MaxLength(200, ErrorMessage = "Kunyomi must not exceed 200 characters")]
        public string? Kunyomi { get; set; }

        [Range(1, 50, ErrorMessage = "Strokes must be between 1 and 50")]
        public int? Strokes { get; set; }

        [MaxLength(10, ErrorMessage = "Level must not exceed 10 characters")]
        public string? Level { get; set; }
    }
}


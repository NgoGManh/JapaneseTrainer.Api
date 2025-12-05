using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Grammar
{
    public class UpdateGrammarMasterRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Meaning { get; set; }

        [MaxLength(500)]
        public string? Formation { get; set; }

        [MaxLength(2000)]
        public string? Usage { get; set; }

        [MaxLength(2000)]
        public string? Example { get; set; }

        [MaxLength(20)]
        public string? Level { get; set; }

        [MaxLength(200)]
        public string? Tags { get; set; }
    }
}

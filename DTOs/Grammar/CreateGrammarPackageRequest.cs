using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Grammar
{
    public class CreateGrammarPackageRequest
    {
        [Required]
        public Guid GrammarMasterId { get; set; }

        [Required]
        public Guid PackageId { get; set; }

        [MaxLength(200)]
        public string? CustomTitle { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Grammar
{
    public class UpdateGrammarPackageRequest
    {
        [MaxLength(200)]
        public string? CustomTitle { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}

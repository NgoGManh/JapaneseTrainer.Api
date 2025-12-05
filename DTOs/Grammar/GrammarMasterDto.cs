using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Grammar
{
    public class GrammarMasterDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Meaning { get; set; }
        public string? Formation { get; set; }
        public string? Usage { get; set; }
        public string? Example { get; set; }
        public string? Level { get; set; }
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

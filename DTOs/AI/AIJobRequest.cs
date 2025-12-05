using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.DTOs.AI
{
    public class AIJobRequest
    {
        [Required]
        public AIJobType Type { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Payload { get; set; } = string.Empty; // JSON string
    }
}


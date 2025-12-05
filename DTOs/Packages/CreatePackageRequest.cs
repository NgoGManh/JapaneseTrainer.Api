using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class CreatePackageRequest
    {
        public Guid? OwnerId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsPublic { get; set; } = true;

        [MaxLength(20)]
        public string? Level { get; set; }

        [MaxLength(200)]
        public string? Tags { get; set; }
    }
}

namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class PackageDto
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public string? Level { get; set; }
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<LessonDto> Lessons { get; set; } = new();
    }
}

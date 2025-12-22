namespace JapaneseTrainer.Api.DTOs.Packages
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<Guid> ItemIds { get; set; } = new();
        public List<Guid> GrammarMasterIds { get; set; } = new();
        public List<Guid> KanjiIds { get; set; } = new();
    }
}

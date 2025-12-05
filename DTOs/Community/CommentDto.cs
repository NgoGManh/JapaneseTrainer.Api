namespace JapaneseTrainer.Api.DTOs.Community
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

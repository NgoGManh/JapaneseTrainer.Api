namespace JapaneseTrainer.Api.DTOs.Grammar
{
    public class GrammarPackageDto
    {
        public Guid Id { get; set; }
        public Guid GrammarMasterId { get; set; }
        public Guid PackageId { get; set; }
        public string? CustomTitle { get; set; }
        public string? Notes { get; set; }

        // Thông tin master gắn kèm (optional, dùng khi muốn fetch chi tiết)
        public GrammarMasterDto? Master { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

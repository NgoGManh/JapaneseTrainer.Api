using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Bản ngữ pháp gắn với một package/bộ bài học cụ thể
    /// </summary>
    public class GrammarPackage : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public Guid GrammarMasterId { get; set; }

        [Required]
        public Guid PackageId { get; set; } // sẽ link tới model Package ở Feature E

        [MaxLength(200)]
        public string? CustomTitle { get; set; } // tiêu đề tuỳ biến trong package

        [MaxLength(2000)]
        public string? Notes { get; set; } // ghi chú riêng cho package/bài học

        // Navigation
        [ForeignKey(nameof(GrammarMasterId))]
        public GrammarMaster GrammarMaster { get; set; } = null!;

        // Package navigation sẽ được thêm sau khi có model Package
        // public Package Package { get; set; } = null!;
    }
}
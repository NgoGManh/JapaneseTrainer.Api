using System.ComponentModel.DataAnnotations;

namespace JapaneseTrainer.Api.Models
{
    /// <summary>
    /// Ngữ pháp chuẩn (master), không phụ thuộc package
    /// </summary>
    public class GrammarMaster : AuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty; // ví dụ: "〜ている", "〜なければならない"

        [MaxLength(500)]
        public string? Meaning { get; set; } // nghĩa tiếng Việt / tiếng Anh

        [MaxLength(500)]
        public string? Formation { get; set; } // cách kết hợp: Vている, Vなければならない, ...

        [MaxLength(2000)]
        public string? Usage { get; set; } // mô tả cách dùng chi tiết

        [MaxLength(2000)]
        public string? Example { get; set; } // ví dụ tổng quát (có thể tách sau nếu cần)

        [MaxLength(20)]
        public string? Level { get; set; } // N5..N1 hoặc custom level

        [MaxLength(200)]
        public string? Tags { get; set; } // CSV: polite, casual, JLPT-N3, ...

        // Navigation
        public ICollection<GrammarPackage> GrammarPackages { get; set; } = new List<GrammarPackage>();
    }
}
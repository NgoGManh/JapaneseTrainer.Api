using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace JapaneseTrainer.Api.Data.Seeds
{
    /// <summary>
    /// Seed initial Package, Lesson, and Grammar data for testing
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1. Kiểm tra nếu đã có Package rồi thì thôi
            if (await context.Packages.AnyAsync())
            {
                return;
            }

            var now = DateTime.UtcNow;

            // --- BƯỚC 1: TẠO PACKAGE (BỘ BÀI HỌC) ---
            var n5Package = new Package
            {
                Id = Guid.NewGuid(),
                Title = "Minna no Nihongo I (N5)",
                Description = "Giáo trình tiếng Nhật sơ cấp phổ biến nhất. Bao gồm từ vựng và ngữ pháp bài 1-25.",
                Level = "N5",
                IsPublic = true,
                Tags = "minna,basic,textbook",
                CreatedAt = now,
                UpdatedAt = now
            };

            // --- BƯỚC 2: TẠO LESSONS (BÀI HỌC CON) ---
            var lesson1 = new Lesson
            {
                Id = Guid.NewGuid(),
                Title = "Bài 1: Giới thiệu bản thân",
                Description = "Chào hỏi, tên, tuổi, nghề nghiệp, quốc tịch.",
                Order = 1,
                PackageId = n5Package.Id, // Link vào Package trên
                CreatedAt = now,
                UpdatedAt = now
            };

            var lesson2 = new Lesson
            {
                Id = Guid.NewGuid(),
                Title = "Bài 2: Đồ vật xung quanh",
                Description = "Cái này, cái đó, cái kia. Sở hữu đồ vật.",
                Order = 2,
                PackageId = n5Package.Id,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Add Package và Lesson vào context (EF tự hiểu Lesson thuộc Package)
            n5Package.Lessons.Add(lesson1);
            n5Package.Lessons.Add(lesson2);
            await context.Packages.AddAsync(n5Package);

            // --- BƯỚC 3: TẠO GRAMMAR MASTER (NGỮ PHÁP GỐC) ---
            // Tạo 2 ngữ pháp mẫu cho Bài 1 và Bài 2

            var g1 = new GrammarMaster
            {
                Id = Guid.NewGuid(),
                Title = "～は～です",
                Meaning = "A là B",
                Formation = "N1 + は + N2 + です",
                Usage = "Dùng để giới thiệu tên, nghề nghiệp. Trợ từ は đọc là wa.",
                Example = "私は学生です。(Tôi là sinh viên)",
                Level = "N5",
                Tags = "basic,copula,polite",
                CreatedAt = now,
                UpdatedAt = now
            };

            var g2 = new GrammarMaster
            {
                Id = Guid.NewGuid(),
                Title = "これ・それ・あれ",
                Meaning = "Cái này, cái đó, cái kia",
                Formation = "これ/それ/あれ + は + N + です",
                Usage = "Chỉ định đồ vật. Kore (gần người nói), Sore (gần người nghe), Are (xa cả hai).",
                Example = "これは本です。(Cái này là quyển sách)",
                Level = "N5",
                Tags = "demonstrative,pronoun,basic",
                CreatedAt = now,
                UpdatedAt = now
            };

            await context.GrammarMasters.AddRangeAsync(g1, g2);

            // --- BƯỚC 4: LIÊN KẾT NGỮ PHÁP VÀO PACKAGE (GRAMMAR PACKAGE) ---
            // Bảng này giúp ta biết Package N5 chứa những ngữ pháp nào

            var gp1 = new GrammarPackage
            {
                Id = Guid.NewGuid(),
                PackageId = n5Package.Id,
                GrammarMasterId = g1.Id,
                CustomTitle = "Cấu trúc khẳng định (Bài 1)",
                Notes = "Mẫu câu quan trọng nhất.",
                CreatedAt = now,
                UpdatedAt = now
            };

            var gp2 = new GrammarPackage
            {
                Id = Guid.NewGuid(),
                PackageId = n5Package.Id,
                GrammarMasterId = g2.Id,
                CustomTitle = "Đại từ chỉ định (Bài 2)",
                Notes = "Lưu ý vị trí của vật.",
                CreatedAt = now,
                UpdatedAt = now
            };

            await context.GrammarPackages.AddRangeAsync(gp1, gp2);

            // --- LƯU TẤT CẢ ---
            await context.SaveChangesAsync();
        }
    }
}


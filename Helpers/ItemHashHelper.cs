using System.Security.Cryptography;
using System.Text;

namespace JapaneseTrainer.Api.Helpers
{
    /// <summary>
    /// Helper class for generating HashKey for Items to prevent duplicates
    /// </summary>
    public static class ItemHashHelper
    {
        /// <summary>
        /// Generate HashKey from Japanese text and Reading
        /// </summary>
        /// <param name="japanese">Japanese text (kanji or kana)</param>
        /// <param name="reading">Reading (hiragana/katakana)</param>
        /// <returns>32-character hex string hash</returns>
        public static string GenerateHashKey(string japanese, string? reading = null)
        {
            // Normalize: trim whitespace and use consistent format
            var normalizedJapanese = (japanese ?? string.Empty).Trim();
            var normalizedReading = (reading ?? string.Empty).Trim();

            // Create hash input: "japanese_reading"
            var hashInput = $"{normalizedJapanese}_{normalizedReading}";

            // Generate SHA256 hash
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));
            
            // Convert to hex string and take first 32 characters
            return Convert.ToHexString(hashBytes).Substring(0, 32);
        }
    }
}


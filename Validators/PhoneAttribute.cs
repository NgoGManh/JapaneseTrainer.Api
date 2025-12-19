using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JapaneseTrainer.Api.Validators
{
    /// <summary>
    /// Validates that a string is a valid phone number.
    /// Supports international format with optional + prefix.
    /// </summary>
    public class PhoneAttribute : ValidationAttribute
    {
        private static readonly Regex PhoneRegex = new(
            @"^\+?[1-9]\d{1,14}$", // E.164 format
            RegexOptions.Compiled);

        public PhoneAttribute()
        {
            ErrorMessage = "The {0} field must be a valid phone number.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null || value is not string phone)
            {
                return true; // Let Required attribute handle null/empty
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                return true; // Let Required attribute handle empty
            }

            // Remove common formatting characters
            var cleaned = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(".", "");

            return PhoneRegex.IsMatch(cleaned);
        }
    }
}


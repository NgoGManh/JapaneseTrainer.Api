using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JapaneseTrainer.Api.Validators
{
    /// <summary>
    /// Validates that a string is a valid email address.
    /// </summary>
    public class EmailAttribute : ValidationAttribute
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public EmailAttribute()
        {
            ErrorMessage = "The {0} field must be a valid email address.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null || value is not string email)
            {
                return true; // Let Required attribute handle null/empty
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return true; // Let Required attribute handle empty
            }

            return EmailRegex.IsMatch(email) && email.Length <= 254; // RFC 5321
        }
    }
}


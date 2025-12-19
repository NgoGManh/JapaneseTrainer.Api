using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JapaneseTrainer.Api.Validators
{
    /// <summary>
    /// Validates that a string is a valid URL.
    /// </summary>
    public class UrlAttribute : ValidationAttribute
    {
        private static readonly Regex UrlRegex = new(
            @"^https?://([\w-]+\.)+[\w-]+(/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UrlAttribute()
        {
            ErrorMessage = "The {0} field must be a valid URL.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null || value is not string url)
            {
                return true; // Let Required attribute handle null/empty
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return true; // Let Required attribute handle empty
            }

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}


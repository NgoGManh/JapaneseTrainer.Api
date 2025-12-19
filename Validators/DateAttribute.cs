using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace JapaneseTrainer.Api.Validators
{
    /// <summary>
    /// Validates that a string matches the date format "yyyy-MM-dd".
    /// </summary>
    public class DateAttribute : ValidationAttribute
    {
        private const string DateFormat = "yyyy-MM-dd";

        public DateAttribute()
        {
            ErrorMessage = $"The {{0}} field must be a valid date in format {DateFormat}.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null || value is not string dateStr)
            {
                return true; // Let Required attribute handle null/empty
            }

            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return true; // Let Required attribute handle empty
            }

            return DateTime.TryParseExact(
                dateStr,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }
    }
}


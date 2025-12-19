using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace JapaneseTrainer.Api.Validators
{
    /// <summary>
    /// Validates that a string matches the datetime format "yyyy-MM-dd HH:mm:ss".
    /// </summary>
    public class DateTimeAttribute : ValidationAttribute
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public DateTimeAttribute()
        {
            ErrorMessage = $"The {{0}} field must be a valid datetime in format {DateTimeFormat}.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null || value is not string dateTimeStr)
            {
                return true; // Let Required attribute handle null/empty
            }

            if (string.IsNullOrWhiteSpace(dateTimeStr))
            {
                return true; // Let Required attribute handle empty
            }

            return System.DateTime.TryParseExact(
                dateTimeStr,
                DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }
    }
}


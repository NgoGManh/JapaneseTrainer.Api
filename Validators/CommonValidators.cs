using System.Text.RegularExpressions;
using JapaneseTrainer.Api.Exceptions;

namespace JapaneseTrainer.Api.Validators
{
    /// <summary>
    /// Common validation functions similar to Python validators
    /// </summary>
    public static class CommonValidators
    {
        /// <summary>
        /// Validates whether a string is a valid GUID (ObjectId equivalent)
        /// </summary>
        public static string CheckObjectId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new InvalidObjectIdException(id);
            }

            if (!Guid.TryParse(id, out _))
            {
                throw new InvalidObjectIdException(id);
            }

            return id;
        }

        /// <summary>
        /// Validates whether a string is a valid email address
        /// </summary>
        public static string CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidEmailException(email);
            }

            // Simple email regex validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(email))
            {
                throw new InvalidEmailException(email);
            }

            return email;
        }

        /// <summary>
        /// Validates whether a string is a valid phone number
        /// Supports international format: +[country code][number]
        /// </summary>
        public static string CheckPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new InvalidPhoneException(phone);
            }

            // Remove spaces, dashes, parentheses for validation
            var cleaned = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Basic phone validation: digits only, optionally starting with +
            var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
            if (!phoneRegex.IsMatch(cleaned))
            {
                throw new InvalidPhoneException(phone);
            }

            return phone;
        }

        /// <summary>
        /// Validates whether a string matches the date format "YYYY-MM-DD"
        /// </summary>
        public static string CheckDateFormat(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                throw new InvalidDateException(date);
            }

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                throw new InvalidDateException(date);
            }

            return date;
        }

        /// <summary>
        /// Validates whether a string matches the datetime format "YYYY-MM-DD HH:mm:ss"
        /// </summary>
        public static string CheckDateTimeStr(string dateTime)
        {
            if (string.IsNullOrWhiteSpace(dateTime))
            {
                throw new InvalidDateTimeException(dateTime);
            }

            if (!DateTime.TryParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _))
            {
                throw new InvalidDateTimeException(dateTime);
            }

            return dateTime;
        }

        /// <summary>
        /// Validates whether a string is a valid URL
        /// </summary>
        public static string CheckUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidUrlException(url);
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidUrlException(url);
            }

            return url;
        }
    }
}


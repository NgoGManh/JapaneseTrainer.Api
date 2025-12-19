using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Validators;

namespace JapaneseTrainer.Api.Common
{
    /// <summary>
    /// Type aliases with validation (similar to Python's Annotated types with AfterValidator)
    /// </summary>
    public static class TypeAliases
    {
        /// <summary>
        /// Validated email string (must be valid email format)
        /// </summary>
        public class EmailStr
        {
            [Validators.EmailAttribute]
            public string Value { get; set; } = string.Empty;

            public static implicit operator string(EmailStr email) => email.Value;
            public static implicit operator EmailStr(string email) => new() { Value = email };
        }

        /// <summary>
        /// Validated phone string (must be valid phone format)
        /// </summary>
        public class PhoneStr
        {
            [Validators.PhoneAttribute]
            public string Value { get; set; } = string.Empty;

            public static implicit operator string(PhoneStr phone) => phone.Value;
            public static implicit operator PhoneStr(string phone) => new() { Value = phone };
        }

        /// <summary>
        /// Validated date string (format: yyyy-MM-dd)
        /// </summary>
        public class DateStr
        {
            [DateAttribute]
            public string Value { get; set; } = string.Empty;

            public static implicit operator string(DateStr date) => date.Value;
            public static implicit operator DateStr(string date) => new() { Value = date };
        }

        /// <summary>
        /// Validated datetime string (format: yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public class DateTimeStr
        {
            [DateTimeAttribute]
            public string Value { get; set; } = string.Empty;

            public static implicit operator string(DateTimeStr dateTime) => dateTime.Value;
            public static implicit operator DateTimeStr(string dateTime) => new() { Value = dateTime };
        }

        /// <summary>
        /// Validated URL string (must be valid HTTP/HTTPS URL)
        /// </summary>
        public class UrlStr
        {
            [Validators.UrlAttribute]
            public string Value { get; set; } = string.Empty;

            public static implicit operator string(UrlStr url) => url.Value;
            public static implicit operator UrlStr(string url) => new() { Value = url };
        }
    }
}


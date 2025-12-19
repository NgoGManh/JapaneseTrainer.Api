using System.ComponentModel.DataAnnotations;
using JapaneseTrainer.Api.Validators;

namespace JapaneseTrainer.Api.Attributes
{
    /// <summary>
    /// Validates that a string is a valid GUID (ObjectId equivalent)
    /// </summary>
    public class ObjectIdAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            try
            {
                CommonValidators.CheckObjectId(value.ToString()!);
                return ValidationResult.Success;
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }

    /// <summary>
    /// Validates that a string is a valid email address
    /// </summary>
    public class EmailFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            try
            {
                CommonValidators.CheckEmail(value.ToString()!);
                return ValidationResult.Success;
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }

    /// <summary>
    /// Validates that a string is a valid phone number
    /// </summary>
    public class PhoneFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            try
            {
                CommonValidators.CheckPhone(value.ToString()!);
                return ValidationResult.Success;
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }

    /// <summary>
    /// Validates that a string matches the date format "YYYY-MM-DD"
    /// </summary>
    public class DateFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            try
            {
                CommonValidators.CheckDateFormat(value.ToString()!);
                return ValidationResult.Success;
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }

    /// <summary>
    /// Validates that a string matches the datetime format "YYYY-MM-DD HH:mm:ss"
    /// </summary>
    public class DateTimeFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            try
            {
                CommonValidators.CheckDateTimeStr(value.ToString()!);
                return ValidationResult.Success;
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }

    /// <summary>
    /// Validates that a string is a valid URL
    /// </summary>
    public class UrlFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let Required attribute handle null checks
            }

            try
            {
                CommonValidators.CheckUrl(value.ToString()!);
                return ValidationResult.Success;
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }
    }
}


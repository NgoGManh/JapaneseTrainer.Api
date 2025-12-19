namespace JapaneseTrainer.Api.Exceptions
{
    /// <summary>
    /// Base exception class for application errors with error codes
    /// Similar to CoreErrorCode in Python FastAPI
    /// </summary>
    public abstract class ErrorCodeException : Exception
    {
        public string ErrorCode { get; }
        public object? Details { get; }

        protected ErrorCodeException(string errorCode, string message, object? details = null)
            : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }

    /// <summary>
    /// Invalid ObjectId (Guid) exception
    /// </summary>
    public class InvalidObjectIdException : ErrorCodeException
    {
        public InvalidObjectIdException(string id)
            : base("INVALID_OBJECT_ID", $"Invalid ObjectId format: {id}", new { id })
        {
        }
    }

    /// <summary>
    /// Invalid email format exception
    /// </summary>
    public class InvalidEmailException : ErrorCodeException
    {
        public InvalidEmailException(string email)
            : base("INVALID_EMAIL", $"Invalid email format: {email}", new { email })
        {
        }
    }

    /// <summary>
    /// Invalid phone number format exception
    /// </summary>
    public class InvalidPhoneException : ErrorCodeException
    {
        public InvalidPhoneException(string phone)
            : base("INVALID_PHONE", $"Invalid phone number format: {phone}", new { phone })
        {
        }
    }

    /// <summary>
    /// Invalid date format exception
    /// </summary>
    public class InvalidDateException : ErrorCodeException
    {
        public InvalidDateException(string date)
            : base("INVALID_DATE", $"Invalid date format. Expected: YYYY-MM-DD. Got: {date}", new { date })
        {
        }
    }

    /// <summary>
    /// Invalid datetime format exception
    /// </summary>
    public class InvalidDateTimeException : ErrorCodeException
    {
        public InvalidDateTimeException(string dateTime)
            : base("INVALID_DATETIME", $"Invalid datetime format. Expected: YYYY-MM-DD HH:mm:ss. Got: {dateTime}", new { dateTime })
        {
        }
    }

    /// <summary>
    /// Invalid URL format exception
    /// </summary>
    public class InvalidUrlException : ErrorCodeException
    {
        public InvalidUrlException(string url)
            : base("INVALID_URL", $"Invalid URL format: {url}", new { url })
        {
        }
    }
}


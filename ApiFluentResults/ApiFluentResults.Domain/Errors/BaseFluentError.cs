using FluentResults;

namespace ApiFluentResults.Domain.Errors
{
    public abstract class BaseFluentError : Error
    {
        public string ErrorCode { get; set; }

        public BaseFluentError(string message, string errorCode = "BUSINESS_ERROR")
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}

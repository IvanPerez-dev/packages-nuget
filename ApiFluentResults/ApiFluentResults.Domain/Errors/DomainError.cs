using FluentResults;

namespace ApiFluentResults.Domain.Errors
{
    public class DomainError : Error
    {
        public string ErrorCode { get; set; }

        public DomainError(string message, string errorCode = "BUSINESS_ERROR")
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}

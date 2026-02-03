using FluentResults;

namespace ApiFluentResults.Domain.Errors
{
    public class DomainError : BaseFluentError
    {
        public DomainError(string message, string errorCode = "BUSINESS_ERROR")
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}

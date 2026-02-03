namespace ApiFluentResults.Domain.Errors
{
    public sealed class ValidationError : BaseFluentError
    {
        public ValidationError(string message)
            : base(message, "VALIDATION_ERROR") { }
    }
}

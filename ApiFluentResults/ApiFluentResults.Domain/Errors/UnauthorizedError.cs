namespace ApiFluentResults.Domain.Errors
{
    public sealed class UnauthorizedError : BaseFluentError
    {
        public UnauthorizedError(string message)
            : base(message, "UNAUTHORIZED") { }
    }
}

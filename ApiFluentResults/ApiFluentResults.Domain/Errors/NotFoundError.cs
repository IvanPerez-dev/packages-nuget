namespace ApiFluentResults.Domain.Errors
{
    public sealed class NotFoundError : BaseFluentError
    {
        public NotFoundError(string message)
            : base(message, "NOT_FOUND") { }
    }
}

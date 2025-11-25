namespace ApiFluentResults.Domain.Errors
{
    public sealed class UnauthorizedError : DomainError
    {
        public UnauthorizedError(string message)
            : base(message, "UNAUTHORIZED")
        {
            //Metadata.Add(MetadataKeys.ErrorCode, );
        }
    }
}

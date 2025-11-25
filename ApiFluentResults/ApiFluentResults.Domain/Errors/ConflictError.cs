namespace ApiFluentResults.Domain.Errors
{
    public sealed class ConflictError : DomainError
    {
        public ConflictError(string message)
            : base(message, "CONFLICT")
        {
            //Metadata.Add(MetadataKeys.ErrorCode, "CONFLICT");
        }
    }
}

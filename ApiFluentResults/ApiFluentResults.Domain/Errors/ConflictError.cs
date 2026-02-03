namespace ApiFluentResults.Domain.Errors
{
    public sealed class ConflictError : BaseFluentError
    {
        public ConflictError(string message)
            : base(message, "CONFLICT")
        {
            //Metadata.Add(MetadataKeys.ErrorCode, "CONFLICT");
        }
    }
}

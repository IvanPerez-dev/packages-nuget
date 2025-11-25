namespace ApiFluentResults.Domain.Errors
{
    public sealed class ValidationError : DomainError
    {
        public ValidationError(string message)
            : base(message, "VALIDATION_ERROR")
        {
            //Metadata.Add(MetadataKeys.ErrorCode, "VALIDATION_ERROR");
        }
    }
}

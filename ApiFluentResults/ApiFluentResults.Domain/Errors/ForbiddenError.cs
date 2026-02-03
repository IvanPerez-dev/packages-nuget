namespace ApiFluentResults.Domain.Errors
{
    public sealed class ForbiddenError : BaseFluentError
    {
        public ForbiddenError(string message)
            : base(message, "FORBIDDEN")
        {
            //Metadata.Add(MetadataKeys.ErrorCode, "FORBIDDEN");
        }
    }
}

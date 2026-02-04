using ApiFluentResults.Domain.Errors;

namespace ApiFluentResult.Services
{
    public sealed class MaximumLengthInvalidError : BaseFluentError
    {
        public MaximumLengthInvalidError(int length)
            : base($"Maximum length is {length}", "MAX_LENGTH_INVALID") { }
    }
}

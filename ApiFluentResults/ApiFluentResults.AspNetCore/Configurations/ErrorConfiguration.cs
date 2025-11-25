namespace ApiFluentResults.AspNetCore.Configurations
{
    public static class ErrorConfiguration
    {
        public static Dictionary<int, ErrorTypeInfo> ErrorTypes = new()
        {
            [400] = new ErrorTypeInfo(
                Title: "Bad Request",
                Type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail: "The request could not be understood by the server due to malformed syntax."
            ),
            [401] = new ErrorTypeInfo(
                Title: "Unauthorized",
                Type: "https://tools.ietf.org/html/rfc7235#section-3.1",
                Detail: "The request requires user authentication."
            ),
            [403] = new ErrorTypeInfo(
                Title: "Forbidden",
                Type: "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Detail: "The server understood the request, but is refusing to fulfill it."
            ),
            [404] = new ErrorTypeInfo(
                Title: "Resource Not Found",
                Type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Detail: "The requested resource could not be found on the server."
            ),
            [409] = new ErrorTypeInfo(
                Title: "Conflict",
                Type: "https://tools.ietf.org/html/rfc4918#section-11.5",
                Detail: "The request could not be completed due to a conflict with the current state of the resource."
            ),
            [422] = new ErrorTypeInfo(
                Title: "Business Rule Violation",
                Type: "https://tools.ietf.org/html/rfc4918#section-11.2",
                Detail: "The request was well-formed but unable to be processed due to business logic constraints."
            ),
            [500] = new ErrorTypeInfo(
                Title: "Internal Server Error",
                Type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Detail: "The server encountered an unexpected condition that prevented it from fulfilling the request."
            ),
        };

        public static ErrorTypeInfo GetErrorInfo(int statusCode)
        {
            return ErrorTypes.GetValueOrDefault(statusCode, DefaultError);
        }

        private static readonly ErrorTypeInfo DefaultError = new(
            Title: "An error occurred",
            Type: "https://tools.ietf.org/html/rfc2616#section-10",
            Detail: "An unexpected error occurred while processing the request."
        );
    }

    public record ErrorTypeInfo(string Title, string Type, string Detail);
}

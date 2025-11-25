using ApiFluentResults.AspNetCore.Configurations;
using ApiFluentResults.Domain.Constants;
using ApiFluentResults.Domain.DTOs;
using ApiFluentResults.Domain.Errors;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Osom.FluentRestult.API.Extensions
{
    public class ResultMapper<T>
    {
        private readonly Result<T> _result;
        private readonly ControllerBase _controller;
        private readonly Dictionary<Type, Func<Error, IActionResult>> _errorMappers = new();
        private Func<IActionResult>? _successFactory;

        internal ResultMapper(Result<T> result, ControllerBase controller)
        {
            _result = result;
            _controller = controller;
            ConfigureDefaultErrorMapper();
        }

        public ResultMapper<T> OkEmpty()
        {
            _successFactory = () => _controller.Ok();
            return this;
        }

        /// <summary>
        /// Configura 200 OK devolviendo result.Value tal cual (sin modificaciones).
        /// Úsalo cuando el DTO ya viene listo de Application.
        /// </summary>
        public ResultMapper<T> Ok() // si no pasas nada → usa result.Value
        {
            _successFactory = () =>
                _result.Value is null ? _controller.Ok() : _controller.Ok(_result.Value);
            return this;
        }

        /// <summary>
        /// Configura 201 Created devolviendo result.Value y Location header automática basada en Id.
        /// Tambien puedes pasar un uri personalizado.
        /// </summary>
        public ResultMapper<T> Created(string? uri = null)
        {
            string location = uri ?? BuildLocationUri();
            _successFactory = () => _controller.Created(location, _result.Value);
            return this;
        }

        public ResultMapper<T> CreatedAtAction(string actionName, object? routeValues = null)
        {
            _successFactory = () =>
                _controller.CreatedAtAction(actionName, routeValues, _result.Value);
            return this;
        }

        public ResultMapper<T> NoContent()
        {
            _successFactory = () => _controller.NoContent();
            return this;
        }

        public ResultMapper<T> Accepted(string? uri = null, T? value = default)
        {
            var body = value ?? _result.ValueOrDefault;
            _successFactory = uri is null
                ? () => _controller.Accepted(body)
                : () => _controller.Accepted(uri, body);
            return this;
        }

        public ResultMapper<T> ConflictFor<TErr>()
            where TErr : Error => MapErrorToStatus<TErr>(409);

        public ResultMapper<T> NotFoundFor<TErr>()
            where TErr : Error => MapErrorToStatus<TErr>(404);

        public ResultMapper<T> BadRequestFor<TErr>()
            where TErr : Error => MapErrorToStatus<TErr>(400);

        public ResultMapper<T> UnprocessableFor<TErr>()
            where TErr : Error => MapErrorToStatus<TErr>(422);

        public ResultMapper<T> UnauthorizedFor<TErr>()
            where TErr : Error => MapErrorToStatus<TErr>(401);

        public ResultMapper<T> ForbiddenFor<TErr>()
            where TErr : Error => MapErrorToStatus<TErr>(403);

        private ResultMapper<T> MapErrorToStatus<TErr>(int statusCode)
            where TErr : Error
        {
            _errorMappers[typeof(TErr)] = err => CreateStatusCodeResult(err, statusCode);
            return this;
        }

        private void ConfigureDefaultErrorMapper()
        {
            MapErrorToStatus<NotFoundError>(StatusCodes.Status404NotFound);
            MapErrorToStatus<ConflictError>(StatusCodes.Status409Conflict);
            MapErrorToStatus<ValidationError>(StatusCodes.Status400BadRequest);
            MapErrorToStatus<UnauthorizedError>(StatusCodes.Status401Unauthorized);
            MapErrorToStatus<ForbiddenError>(StatusCodes.Status403Forbidden);
            MapErrorToStatus<DomainError>(StatusCodes.Status422UnprocessableEntity);
        }

        private string BuildLocationUri()
        {
            var requestPath = _controller.Url.ActionContext.HttpContext.Request.Path;
            var idProperty = _result?.ValueOrDefault?.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(
                    _result is null ? default(T) : _result.ValueOrDefault
                );
                return $"{requestPath}/{idValue}";
            }
            return requestPath;
        }

        private IActionResult CreateStatusCodeResult(Error error, int statusCode)
        {
            var problemDetails = Problem(error, statusCode);

            return statusCode switch
            {
                400 => _controller.BadRequest(problemDetails),
                401 => _controller.Unauthorized(problemDetails),
                404 => _controller.NotFound(problemDetails),
                409 => _controller.Conflict(problemDetails),
                422 => _controller.UnprocessableEntity(problemDetails),
                _ => _controller.StatusCode(statusCode, problemDetails),
            };
        }

        public IActionResult Build()
        {
            if (_result.IsSuccess)
            {
                if (_successFactory is null)
                    throw new InvalidOperationException(
                        "Debes configurar una respuesta de éxito: .Ok(), .Created(), .NoContent(), etc."
                    );
                return _successFactory();
            }

            var error = _result.Errors.FirstOrDefault() as Error;
            if (_errorMappers.TryGetValue(error!.GetType(), out var mapper))
                return mapper(error);

            // Fallback genérico
            return _controller.UnprocessableEntity(
                Problem(error, StatusCodes.Status422UnprocessableEntity)
            );
        }

        private CustomProblemDetails Problem(Error error, int status)
        {
            var errorTypeInfo = ErrorConfiguration.GetErrorInfo(status);

            string errorCode = "UNKNOWN_ERROR";
            if (error is DomainError domainError)
            {
                errorCode = domainError.ErrorCode;
            }

            var problemDetails = new CustomProblemDetails(
                title: errorTypeInfo.Title,
                detail: error.Message ?? errorTypeInfo.Detail,
                type: errorTypeInfo.Type,
                status: status,
                instance: _controller.HttpContext.Request.Path
            )
            {
                Code = errorCode,
                Metadata = error.Metadata,
            };

            if (
                error.Metadata.TryGetValue(MetadataKeys.ValidationErrors, out var errorObject)
                && errorObject is Dictionary<string, List<string>> validationErrors
            )
            {
                problemDetails.Errors = validationErrors;
            }

            return problemDetails;
        }
    }
}

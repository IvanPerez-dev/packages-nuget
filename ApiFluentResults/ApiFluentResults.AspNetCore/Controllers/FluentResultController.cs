using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Osom.FluentRestult.API.Extensions;

namespace ApiFluentResults.AspNetCore.Controllers
{
    // <summary>
    /// Controlador base que proporciona métodos de mapeo para FluentResults.
    /// Hereda de este para usar .MapResult() fluentemente.
    /// </summary>
    public class FluentResultController : ControllerBase
    {
        protected ResultMapper<T> MapResult<T>(Result<T> result) => new(result, this);

        protected ResultMapper<object> MapResult(Result result) =>
            new(result.ToResult<object>(), this);
    }
}

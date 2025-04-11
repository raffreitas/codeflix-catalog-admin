using FC.Codeflix.Catalog.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FC.Codeflix.Catalog.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _environment;

    public ApiGlobalExceptionFilter(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exception = context.Exception;

        if (_environment.IsDevelopment())
            details.Extensions.Add("StackTrace", exception.StackTrace);

        if (exception is EntityValidationException entityValidationException)
        {
            details.Title = "One or more validation error occurred";
            details.Type = "UnprocessableEntity";
            details.Status = StatusCodes.Status422UnprocessableEntity;
            details.Detail = entityValidationException.Message;
            context.Result = new UnprocessableEntityObjectResult(details);
        }
        else
        {
            details.Title = "An unexpected error occurred";
            details.Type = "InternalServerError";
            details.Status = StatusCodes.Status500InternalServerError;
            details.Detail = exception.Message;
            context.Result = new ObjectResult(details);
        }

        context.HttpContext.Response.StatusCode = (int)details.Status;
        context.ExceptionHandled = true;
    }
}

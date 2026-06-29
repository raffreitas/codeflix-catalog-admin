using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Codeflix.Catalog.Api.Filters;

internal sealed class ApiGlobalExceptionFilter(IHostEnvironment environment) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exception = context.Exception;

        if (environment.IsDevelopment())
            details.Extensions.Add("StackTrace", exception.StackTrace);

        switch (exception)
        {
            case EntityValidationException entityValidationException:
                details.Title = "One or more validation error occurred";
                details.Type = "UnprocessableEntity";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = entityValidationException.Message;
                context.Result = new UnprocessableEntityObjectResult(details);
                break;
            case NotFoundException notFoundException:
                details.Title = "Not Found";
                details.Type = "NotFound";
                details.Status = StatusCodes.Status404NotFound;
                details.Detail = notFoundException.Message;
                context.Result = new NotFoundObjectResult(details);
                break;
            case RelatedAggregateException relatedAggregateException:
                details.Title = "One or more related aggregate error occurred";
                details.Type = "RelatedAggregate";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = relatedAggregateException.Message;
                context.Result = new UnprocessableEntityObjectResult(details);
                break;
            default:
                details.Title = "An unexpected error occurred";
                details.Type = "InternalServerError";
                details.Status = StatusCodes.Status500InternalServerError;
                details.Detail = exception.Message;
                context.Result = new ObjectResult(details);
                break;
        }

        context.HttpContext.Response.StatusCode = (int)details.Status;
        context.ExceptionHandled = true;
    }
}
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CampFitFurDogs.Api.Errors;

public static class ExceptionHandlingMiddleware
{
    public static void Configure(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;

            // unwrap
            while (exception is TargetInvocationException ||
                   exception is InvalidOperationException ||
                   exception is AggregateException)
            {
                if (exception.InnerException == null)
                    break;

                exception = exception.InnerException;
            }

            if (exception is null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Unknown Error",
                    Status = StatusCodes.Status500InternalServerError
                });
                return;
            }

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var problem = new ValidationProblemDetails(errors)
                {
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400"
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }

            // existing mapping for domain + unexpected
            var code = ExceptionToErrorCode.Map(exception);
            var problem2 = ErrorCodeToProblemDetails.Create(code, exception);

            context.Response.StatusCode = problem2.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem2);
        });
    }
}

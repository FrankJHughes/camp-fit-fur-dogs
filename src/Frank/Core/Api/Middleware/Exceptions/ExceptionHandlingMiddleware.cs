using System.Reflection;
using Frank.Core.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Frank.Core.Api.Middleware.Exceptions;

/// <summary>
/// Middleware that intercepts unhandled exceptions, unwraps common wrapper
/// exceptions, resolves the appropriate <see cref="IExceptionHandler"/> from the
/// <see cref="ExceptionHandlerRegistry"/>, and writes a structured
/// <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/> response.
/// <para>
/// This middleware centralizes exception handling for the API, ensuring
/// consistent error formatting, status code assignment, and diagnostic output
/// across all vertical slices.
/// </para>
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ExceptionHandlerRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/>
    /// class.
    /// </summary>
    /// <param name="next">The next middleware in the ASP.NET Core pipeline.</param>
    /// <param name="registry">
    /// The registry responsible for resolving the correct exception handler
    /// based on the thrown exception type.
    /// </param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ExceptionHandlerRegistry registry)
    {
        _next = next;
        _registry = registry;
    }

    /// <summary>
    /// Executes the next middleware and handles any unhandled exceptions by
    /// delegating to the appropriate exception handler.
    /// <para>
    /// Wrapper exceptions such as <see cref="TargetInvocationException"/>,
    /// <see cref="InvalidOperationException"/>, and <see cref="AggregateException"/>
    /// are unwrapped to expose the underlying exception before resolution.
    /// </para>
    /// <para>
    /// The resolved handler produces a <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>
    /// instance, which is serialized to the response with the correct status code.
    /// </para>
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // unwrap common wrapper exceptions
            while (ex is TargetInvocationException or InvalidOperationException or AggregateException
                   && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            var handler = _registry.Resolve(ex);
            var problem = handler.CreateProblemDetails(ex);

            context.Response.StatusCode = problem.Status ?? 500;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}

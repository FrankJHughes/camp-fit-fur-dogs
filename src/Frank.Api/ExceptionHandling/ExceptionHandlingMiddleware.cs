using System.Reflection;
using Frank.Integration.ExceptionHandling;
using Microsoft.AspNetCore.Http;

namespace Frank.Api.ExceptionHandling;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ExceptionHandlerRegistry _registry;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ExceptionHandlerRegistry registry)
    {
        _next = next;
        _registry = registry;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // unwrap
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

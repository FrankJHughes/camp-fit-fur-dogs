using System.Reflection;
using Frank.Api.ExceptionHandling;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ExceptionHandlingRegistry _registry;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ExceptionHandlingRegistry registry)
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

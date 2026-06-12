using System.Reflection;
using Frank.Abstractions.ExceptionHandling;

namespace Frank.Api.ExceptionHandling;

public sealed class ExceptionHandlingRegistry
{
    private readonly IReadOnlyList<IExceptionHandler> _handlers;

    public ExceptionHandlingRegistry(IEnumerable<IExceptionHandler> handlers)
    {
        _handlers = handlers
            .OrderBy(GetOrder)
            .ToArray();
    }

    public IExceptionHandler Resolve(Exception exception)
        => _handlers.First(h => h.CanHandle(exception));

    private static int GetOrder(IExceptionHandler handler)
        => handler.GetType()
            .GetCustomAttribute<ExceptionHandlerAttribute>()?.Order
            ?? 1000;
}

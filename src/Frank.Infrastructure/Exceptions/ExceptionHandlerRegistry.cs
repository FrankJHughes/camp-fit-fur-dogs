using System.Reflection;
using Frank.Abstractions.Exceptions;

namespace Frank.Infrastructure.Problem;

public sealed class ExceptionHandlerRegistry
{
    private readonly IReadOnlyList<IExceptionHandler> _handlers;

    public ExceptionHandlerRegistry(IEnumerable<IExceptionHandler> handlers)
    {
        _handlers = handlers
            .OrderBy(GetOrder)
            .ToArray();
    }

    public IExceptionHandler Resolve(System.Exception exception)
        => _handlers.First(h => h.CanHandle(exception));

    private static int GetOrder(IExceptionHandler handler)
        => handler.GetType()
            .GetCustomAttribute<ExceptionHandlerAttribute>()?.Order
            ?? 1000;
}

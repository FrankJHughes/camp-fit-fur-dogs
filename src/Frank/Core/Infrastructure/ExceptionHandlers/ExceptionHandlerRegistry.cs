using System.Reflection;
using Frank.Core.Application.Abstractions.Exceptions;

namespace Frank.Core.Infrastructure.Exceptions;

/// <summary>
/// Provides ordered resolution of <see cref="IExceptionHandler"/> instances
/// based on their <see cref="ExceptionHandlerAttribute"/> metadata.
/// <para>
/// This registry selects the first handler capable of processing a given
/// exception, ensuring deterministic and predictable exception handling
/// behavior across the application.
/// </para>
/// <para>
/// Handlers may specify an explicit <c>Order</c> via
/// <see cref="ExceptionHandlerAttribute"/>. Handlers without an attribute
/// default to order <c>1000</c>, placing them at the end of the resolution
/// chain.
/// </para>
/// </summary>
public sealed class ExceptionHandlerRegistry
{
    private readonly IReadOnlyList<IExceptionHandler> _handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlerRegistry"/>
    /// class, ordering handlers by their declared <c>Order</c> metadata.
    /// </summary>
    /// <param name="handlers">
    /// The collection of registered exception handlers.
    /// </param>
    public ExceptionHandlerRegistry(IEnumerable<IExceptionHandler> handlers)
    {
        _handlers = handlers
            .OrderBy(GetOrder)
            .ToArray();
    }

    /// <summary>
    /// Resolves the first <see cref="IExceptionHandler"/> capable of handling
    /// the specified exception.
    /// </summary>
    /// <param name="exception">
    /// The exception to resolve a handler for.
    /// </param>
    /// <returns>
    /// The first handler whose <see cref="IExceptionHandler.CanHandle"/> method
    /// returns <c>true</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no handler is capable of handling the exception.
    /// </exception>
    public IExceptionHandler Resolve(System.Exception exception)
        => _handlers.First(h => h.CanHandle(exception));

    /// <summary>
    /// Retrieves the ordering value for a handler based on its
    /// <see cref="ExceptionHandlerAttribute"/>. Handlers without an attribute
    /// default to order <c>1000</c>.
    /// </summary>
    private static int GetOrder(IExceptionHandler handler)
        => handler.GetType()
            .GetCustomAttribute<ExceptionHandlerAttribute>()?.Order
            ?? 1000;
}

using FluentValidation;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Cqrs.Queries;

/// <summary>
/// Dispatches query instances to their corresponding handlers, applying
/// validation and resolving dependencies through the configured service provider.
///
/// <para>
/// The dispatcher performs three responsibilities:
/// <list type="number">
///   <item><description>Resolve and execute all validators associated with the query type.</description></item>
///   <item><description>Resolve the appropriate query handler from the dependency injection container.</description></item>
///   <item><description>Invoke the handler and return its result.</description></item>
/// </list>
/// </para>
///
/// <para>
/// Queries represent read‑side operations and must not mutate state. This
/// dispatcher ensures that all queries follow a consistent, validated, and
/// slice‑aligned execution pipeline.
/// </para>
/// </summary>
public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryDispatcher"/> class.
    /// </summary>
    /// <param name="provider">
    /// The service provider used to resolve validators and query handlers.
    /// </param>
    public QueryDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Dispatches a query, applying validation and invoking the resolved handler.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of response returned by the query handler.
    /// </typeparam>
    /// <param name="query">
    /// The query instance to dispatch.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the operation.
    /// </param>
    /// <returns>
    /// The response produced by the query handler.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown when one or more validators report failures.
    /// </exception>
    public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct)
    {
        // 1. Run validators (if any)
        var validatorType = typeof(IValidator<>).MakeGenericType(query.GetType());
        var validators = _provider.GetServices(validatorType).Cast<object>();

        var context = new ValidationContext<object>(query);

        foreach (var validator in validators)
        {
            var result = await ((IValidator)validator).ValidateAsync(context, ct);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        // 2. Resolve handler
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResponse));

        var handler = (object)_provider.GetRequiredService(handlerType);

        // 3. Execute handler
        return await ((dynamic)handler).HandleAsync((dynamic)query, ct);
    }
}

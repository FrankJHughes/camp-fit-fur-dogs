using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.Cqrs.Queries;

/// <summary>
/// Defines the contract for handling a query in the CQRS (Command Query
/// Responsibility Segregation) pattern.
///
/// <para>
/// Query handlers execute read‑side operations: retrieving data, computing
/// projections, or returning information without modifying application state.
/// The response type is determined by the query and enforced by the handler.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that each query handler is
/// automatically registered into the dependency injection container with a
/// scoped lifetime, concrete type registration, and exactly one implementation.
/// </para>
/// </summary>
/// <typeparam name="TQuery">
/// The type of query being handled.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the value returned by the handler.
/// </typeparam>
[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Processes the specified query asynchronously and returns its typed
    /// response.
    ///
    /// <para>
    /// The handler performs the query’s read‑side behavior. The returned task
    /// completes when the operation has finished or when cancellation is
    /// requested via the provided token.
    /// </para>
    /// </summary>
    /// <param name="query">
    /// The query to be handled.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the handler’s
    /// response value.
    /// </returns>
    Task<TResponse> HandleAsync(TQuery query, CancellationToken ct);
}

namespace Frank.Core.Application.Abstractions.Cqrs.Queries;

/// <summary>
/// Defines the contract for dispatching queries to their corresponding
/// handlers within the CQRS (Command Query Responsibility Segregation)
/// pipeline.
///
/// <para>
/// The <see cref="IQueryDispatcher"/> is responsible for locating the
/// appropriate query handler, invoking it, and returning the typed response.
/// This abstraction decouples query invocation from handler resolution,
/// improving testability and inversion of control.
/// </para>
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query by resolving and invoking its corresponding handler,
    /// returning the typed response produced by the handler.
    ///
    /// <para>
    /// The dispatcher ensures that the correct handler for the given query type
    /// is executed. The returned task completes when the handler finishes
    /// processing the query or when cancellation is requested.
    /// </para>
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the value returned by the query handler.
    /// </typeparam>
    /// <param name="query">
    /// The query to be dispatched.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the handler’s
    /// response value.
    /// </returns>
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}

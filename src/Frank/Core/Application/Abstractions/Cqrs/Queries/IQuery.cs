namespace Frank.Core.Application.Abstractions.Cqrs.Queries;

/// <summary>
/// Represents a query in the CQRS (Command Query Responsibility Segregation)
/// pattern that returns a typed response.
///
/// <para>
/// Queries are read‑side operations: they retrieve data, compute projections,
/// or return information without modifying application state. The response
/// type is defined by the query and enforced by its handler.
/// </para>
/// </summary>
/// <typeparam name="TResponse">
/// The type of the value returned when the query is executed.
/// </typeparam>
public interface IQuery<TResponse> { }

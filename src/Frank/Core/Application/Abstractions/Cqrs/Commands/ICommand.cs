namespace Frank.Core.Application.Abstractions.Cqrs.Commands;

/// <summary>
/// Represents a command in the CQRS (Command Query Responsibility Segregation)
/// pattern.
///
/// <para>
/// A command expresses an intention to change application state. Commands are
/// write‑side operations and are handled by a corresponding command handler.
/// </para>
///
/// <para>
/// This non‑generic form is used when the command does not produce a response
/// value. Handlers typically return <see cref="System.Threading.Tasks.Task"/>.
/// </para>
/// </summary>
public interface ICommand { }

/// <summary>
/// Represents a command in the CQRS pattern that produces a typed response.
///
/// <para>
/// Commands encapsulate state‑changing operations. When a command needs to
/// return a result (for example, an identifier, a status object, or a computed
/// value), the generic form <see cref="ICommand{TResponse}"/> is used.
/// </para>
///
/// <para>
/// The response type is determined by the command and enforced by its handler,
/// ensuring strong typing throughout the command pipeline.
/// </para>
/// </summary>
/// <typeparam name="TResponse">
/// The type of the value returned when the command is executed.
/// </typeparam>
public interface ICommand<TResponse> { }

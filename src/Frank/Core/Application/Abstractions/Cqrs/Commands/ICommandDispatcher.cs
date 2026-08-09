namespace Frank.Core.Application.Abstractions.Cqrs.Commands;

/// <summary>
/// Defines the contract for dispatching commands to their corresponding
/// handlers within the CQRS (Command Query Responsibility Segregation)
/// pipeline.
///
/// <para>
/// The <see cref="ICommandDispatcher"/> is responsible for locating the
/// appropriate command handler, invoking it, and returning the result (for
/// commands that produce one). This abstraction decouples command invocation
/// from handler resolution, enabling testability and inversion of control.
/// </para>
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command that produces a typed response by invoking its
    /// corresponding handler.
    ///
    /// <para>
    /// The dispatcher ensures that the correct handler for the given command
    /// type is resolved and executed. The returned task completes when the
    /// handler finishes processing the command.
    /// </para>
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the value returned by the command handler.
    /// </typeparam>
    /// <param name="command">
    /// The command to be dispatched.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the handler's
    /// response.
    /// </returns>
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct);

    /// <summary>
    /// Dispatches a command that does not produce a response by invoking its
    /// corresponding handler.
    ///
    /// <para>
    /// This overload is used for commands whose handlers perform an action but
    /// do not return a value. The returned task completes when the handler
    /// finishes processing the command.
    /// </para>
    /// </summary>
    /// <param name="command">
    /// The command to be dispatched.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    Task DispatchAsync(ICommand command, CancellationToken ct);
}

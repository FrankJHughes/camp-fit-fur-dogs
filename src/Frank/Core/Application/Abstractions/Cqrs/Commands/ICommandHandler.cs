using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.Cqrs.Commands;

/// <summary>
/// Defines the contract for handling a command in the CQRS (Command Query
/// Responsibility Segregation) pattern.
///
/// <para>
/// This non‑generic handler is used for commands that perform an action but do
/// not return a value. Implementations encapsulate the write‑side behavior
/// associated with the command.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that each command handler is
/// automatically registered into the dependency injection container with a
/// scoped lifetime and exactly one implementation.
/// </para>
/// </summary>
/// <typeparam name="TCommand">
/// The type of command being handled.
/// </typeparam>
[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Processes the specified command asynchronously.
    ///
    /// <para>
    /// The handler performs the command’s write‑side behavior. The returned task
    /// completes when the operation has finished or when cancellation is
    /// requested via the provided token.
    /// </para>
    /// </summary>
    /// <param name="command">
    /// The command to be handled.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    Task HandleAsync(TCommand command, CancellationToken ct);
}

/// <summary>
/// Defines the contract for handling a command in the CQRS pattern that
/// produces a typed response.
///
/// <para>
/// This generic handler is used when the command returns a value, such as an
/// identifier, a status object, or a computed result. The response type is
/// enforced by the command and its handler, ensuring strong typing throughout
/// the pipeline.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that each command handler is
/// automatically registered into the dependency injection container with a
/// scoped lifetime and exactly one implementation.
/// </para>
/// </summary>
/// <typeparam name="TCommand">
/// The type of command being handled.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the value returned by the handler.
/// </typeparam>
[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Processes the specified command asynchronously and returns its response.
    ///
    /// <para>
    /// The handler performs the command’s write‑side behavior and produces a
    /// typed result. The returned task completes when the operation has finished
    /// or when cancellation is requested via the provided token.
    /// </para>
    /// </summary>
    /// <param name="command">
    /// The command to be handled.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the handler’s
    /// response value.
    /// </returns>
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}

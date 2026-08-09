using FluentValidation;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Cqrs.Commands;

/// <summary>
/// Dispatches command instances to their corresponding handlers, applying
/// validation and resolving dependencies through the configured service provider.
///
/// <para>
/// The dispatcher performs three responsibilities:
/// <list type="number">
///   <item><description>Resolve and execute all validators associated with the command type.</description></item>
///   <item><description>Resolve the appropriate command handler from the dependency injection container.</description></item>
///   <item><description>Invoke the handler and return (or await) its result.</description></item>
/// </list>
/// </para>
///
/// <para>
/// This implementation supports both commands that return a response
/// (<see cref="ICommand{TResponse}"/>) and fire‑and‑forget commands
/// (<see cref="ICommand"/>). Validation is optional—commands without validators
/// simply proceed to handler execution.
/// </para>
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
    /// </summary>
    /// <param name="provider">
    /// The service provider used to resolve validators and command handlers.
    /// </param>
    public CommandDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Dispatches a command that produces a response, applying validation and
    /// invoking the resolved handler.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of response returned by the command handler.
    /// </typeparam>
    /// <param name="command">
    /// The command instance to dispatch.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the operation.
    /// </param>
    /// <returns>
    /// The response produced by the command handler.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown when one or more validators report failures.
    /// </exception>
    public async Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct)
    {
        // 1. Run validators (if any)
        var commandType = command.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(commandType);

        var validators = _provider.GetServices(validatorType).Cast<IValidator>().ToList();

        foreach (var validator in validators)
        {
            var contextType = typeof(ValidationContext<>).MakeGenericType(commandType);
            var context = (IValidationContext)Activator.CreateInstance(contextType, command)!;

            var result = await validator.ValidateAsync(context, ct);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        // 2. Resolve handler
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));

        var handler = (object)_provider.GetRequiredService(handlerType);

        // 3. Execute handler
        return await ((dynamic)handler).HandleAsync((dynamic)command, ct);
    }

    /// <summary>
    /// Dispatches a fire‑and‑forget command, applying validation and invoking
    /// the resolved handler.
    /// </summary>
    /// <param name="command">
    /// The command instance to dispatch.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the operation.
    /// </param>
    /// <exception cref="ValidationException">
    /// Thrown when one or more validators report failures.
    /// </exception>
    public async Task DispatchAsync(ICommand command, CancellationToken ct)
    {
        // 1. Run validators (if any)
        var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
        var validators = _provider.GetServices(validatorType).Cast<object>();

        var context = new ValidationContext<object>(command);

        foreach (var validator in validators)
        {
            var result = await ((IValidator)validator).ValidateAsync(context, ct);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        // 2. Resolve handler
        var handlerType = typeof(ICommandHandler<>)
            .MakeGenericType(command.GetType());

        var handler = (object)_provider.GetRequiredService(handlerType);

        // 3. Execute handler
        await ((dynamic)handler).HandleAsync((dynamic)command, ct);
    }
}

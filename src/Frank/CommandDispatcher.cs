using FluentValidation;
using Frank.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _provider;

    public CommandDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

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


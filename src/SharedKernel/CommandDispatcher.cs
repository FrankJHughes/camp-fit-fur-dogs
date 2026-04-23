using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using SharedKernel.Abstractions;

namespace SharedKernel;

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
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));

        var handler = (object)_provider.GetRequiredService(handlerType);

        // 3. Execute handler
        return await ((dynamic)handler).Handle((dynamic)command, ct);
    }
}


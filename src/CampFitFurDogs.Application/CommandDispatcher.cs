using Microsoft.Extensions.DependencyInjection;

using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _provider;

    public CommandDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task<TResponse> Dispatch<TResponse>(ICommand<TResponse> command, CancellationToken ct)
    {
        // To Do: Avoid using reflection and dynamic in production code.
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));

        var handler = _provider.GetRequiredService(handlerType);

        return ((dynamic)handler).Handle((dynamic)command, ct);
    }
}

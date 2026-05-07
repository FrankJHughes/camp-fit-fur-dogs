using SharedKernel.Abstractions;

namespace SharedKernel.Tests.Fakes;

public sealed class TrackingCommandHandler<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public int CallCount { get; private set; }
    public TCommand? LastCommand { get; private set; }
    public CancellationToken? LastToken { get; private set; }

    public Task<TResponse> HandleAsync(TCommand command, CancellationToken ct)
    {
        CallCount++;
        LastCommand = command;
        LastToken = ct;

        // predictable return for string responses
        if (typeof(TResponse) == typeof(string))
        {
            object result = $"handled: {command}";
            return Task.FromResult((TResponse)result);
        }

        return Task.FromResult(default(TResponse)!);
    }
}

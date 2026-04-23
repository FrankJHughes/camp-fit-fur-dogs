using SharedKernel.Abstractions;

namespace SharedKernel.Tests.Fakes;

public sealed class TrackingFakeStringResponseCommandHandler
    : ICommandHandler<FakeStringResponseCommand, string>
{
    public int CallCount { get; private set; }
    public FakeStringResponseCommand? LastCommand { get; private set; }
    public CancellationToken? LastToken { get; private set; }

    public Task<string> Handle(FakeStringResponseCommand command, CancellationToken ct)
    {
        CallCount++;
        LastCommand = command;
        LastToken = ct;

        object result = $"handled: {command}";
        return Task.FromResult((string)result);
    }
}

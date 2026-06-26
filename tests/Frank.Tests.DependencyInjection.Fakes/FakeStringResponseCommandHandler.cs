using Frank.Abstractions.Command;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class FakeStringResponseCommandHandler
    : ICommandHandler<FakeStringResponseCommand, string>
{
    public Task<string> HandleAsync(FakeStringResponseCommand command, CancellationToken ct)
        => Task.FromResult("ok");
}

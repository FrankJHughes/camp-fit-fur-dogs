using Frank.Abstractions;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class FakeCommandHandler
    : ICommandHandler<FakeCommand, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeCommand command, CancellationToken ct)
        => Task.FromResult(new FakeResponse());
}

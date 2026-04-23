using SharedKernel.Abstractions;

namespace SharedKernel.Tests.DependencyInjection.Fakes;

public sealed class FakeStringResponseCommandHandler
    : ICommandHandler<FakeStringResponseCommand, string>
{
    public Task<string> Handle(FakeStringResponseCommand command, CancellationToken ct)
        => Task.FromResult("ok");
}

using SharedKernel.Abstractions;

namespace SharedKernel.Tests.DependencyInjection.DuplicateCommandHandlers;

public sealed class FirstFakeCommandHandler
    : ICommandHandler<FakeCommand, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeCommand command, CancellationToken ct)
        => Task.FromResult(new FakeResponse("first"));
}

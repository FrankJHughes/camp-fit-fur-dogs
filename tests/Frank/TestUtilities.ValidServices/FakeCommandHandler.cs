using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.TestUtilities.ValidServices;

public sealed class FakeCommandHandler
    : ICommandHandler<FakeCommand, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeCommand command, CancellationToken ct)
        => Task.FromResult(new FakeResponse());
}

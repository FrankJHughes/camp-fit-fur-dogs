using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.TestUtilities.ValidServices;

public sealed class FakeStringResponseCommandHandler
    : ICommandHandler<FakeStringResponseCommand, string>
{
    public Task<string> HandleAsync(FakeStringResponseCommand command, CancellationToken ct)
        => Task.FromResult("ok");
}

using Frank.Core.Application.Abstractions.Command;

namespace Frank.TestUtilities.ValidServices;

public sealed class FakeStringResponseCommandHandler
    : ICommandHandler<FakeStringResponseCommand, string>
{
    public Task<string> HandleAsync(FakeStringResponseCommand command, CancellationToken ct)
        => Task.FromResult("ok");
}

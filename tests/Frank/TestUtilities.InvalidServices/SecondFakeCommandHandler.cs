using Frank.Core.Application.Abstractions.Command;

namespace Frank.TestUtilities.InvalidServices;

public sealed class SecondFakeCommandHandler
    : ICommandHandler<FakeCommand, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeCommand command, CancellationToken ct)
        => Task.FromResult(new FakeResponse("second"));
}

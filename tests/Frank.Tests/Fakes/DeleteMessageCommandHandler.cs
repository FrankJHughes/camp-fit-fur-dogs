using Frank.Abstractions.Command;

namespace Frank.Tests.Fakes;

public class DeleteMessageCommandHandler : ICommandHandler<DeleteMessageCommand>
{
    public Task HandleAsync(DeleteMessageCommand command, CancellationToken ct)
        => Task.CompletedTask;
}

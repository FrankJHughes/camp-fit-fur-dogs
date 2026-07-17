using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.Core.Application.Tests.Fakes;

public class DeleteMessageCommandHandler : ICommandHandler<DeleteMessageCommand>
{
    public Task HandleAsync(DeleteMessageCommand command, CancellationToken ct)
        => Task.CompletedTask;
}

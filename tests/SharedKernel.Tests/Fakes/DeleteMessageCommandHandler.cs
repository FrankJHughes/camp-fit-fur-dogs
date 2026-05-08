using SharedKernel.Abstractions;

namespace SharedKernel.Tests.Fakes;

public class DeleteMessageCommandHandler : ICommandHandler<DeleteMessageCommand>
{
    public Task HandleAsync(DeleteMessageCommand command, CancellationToken ct)
        => Task.CompletedTask;
}

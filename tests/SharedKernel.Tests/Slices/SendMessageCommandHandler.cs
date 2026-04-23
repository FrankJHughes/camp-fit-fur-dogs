
namespace SharedKernel.Tests.Slices;

public sealed class SendMessageCommandHandler
    : ICommandHandler<SendMessageCommand, SendMessageResponse>
{
    public Task<SendMessageResponse> Handle(SendMessageCommand command, CancellationToken ct)
        => Task.FromResult(new SendMessageResponse(!string.IsNullOrWhiteSpace(command.Text)));
}


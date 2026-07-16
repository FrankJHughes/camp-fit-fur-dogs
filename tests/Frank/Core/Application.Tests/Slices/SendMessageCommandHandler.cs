
using Frank.Core.Application.Abstractions.Command;

namespace Frank.Core.Application.Tests.Slices;

public sealed class SendMessageCommandHandler
    : ICommandHandler<SendMessageCommand, SendMessageResponse>
{
    public Task<SendMessageResponse> HandleAsync(SendMessageCommand command, CancellationToken ct)
        => Task.FromResult(new SendMessageResponse(!string.IsNullOrWhiteSpace(command.Text)));
}


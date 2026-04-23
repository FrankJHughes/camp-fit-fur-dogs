
namespace SharedKernel.Tests.Slices;

public sealed record SendMessageCommand(string Text)
    : ICommand<SendMessageResponse>;


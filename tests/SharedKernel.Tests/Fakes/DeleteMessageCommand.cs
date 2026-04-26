using SharedKernel.Abstractions;

namespace SharedKernel.Tests.Fakes;

public record DeleteMessageCommand(string MessageId) : ICommand;

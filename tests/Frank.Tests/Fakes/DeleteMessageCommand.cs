using Frank.Abstractions.Command;

namespace Frank.Tests.Fakes;

public record DeleteMessageCommand(string MessageId) : ICommand;

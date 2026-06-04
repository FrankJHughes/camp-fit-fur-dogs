using Frank.Abstractions;

namespace Frank.Tests.Fakes;

public record DeleteMessageCommand(string MessageId) : ICommand;

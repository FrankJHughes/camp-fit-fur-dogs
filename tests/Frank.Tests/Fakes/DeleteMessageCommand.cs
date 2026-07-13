using Frank.Core.Application.Abstractions.Command;

namespace Frank.Tests.Fakes;

public record DeleteMessageCommand(string MessageId) : ICommand;

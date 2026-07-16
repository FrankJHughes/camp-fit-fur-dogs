using Frank.Core.Application.Abstractions.Command;

namespace Frank.Core.Application.Tests.Fakes;

public record DeleteMessageCommand(string MessageId) : ICommand;

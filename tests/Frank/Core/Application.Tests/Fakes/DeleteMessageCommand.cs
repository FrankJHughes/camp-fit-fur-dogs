using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.Core.Application.Tests.Fakes;

public record DeleteMessageCommand(string MessageId) : ICommand;

using Frank.Abstractions.Command;

namespace Frank.Tests.DependencyInjection.DuplicateCommandHandlers;

public sealed record FakeCommand(string Value) : ICommand<FakeResponse>;

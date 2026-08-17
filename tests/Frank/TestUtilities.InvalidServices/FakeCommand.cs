using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.TestUtilities.InvalidServices;

public sealed record FakeCommand(string Value) : ICommand<FakeResponse>;

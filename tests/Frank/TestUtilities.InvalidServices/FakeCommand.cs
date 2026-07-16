using Frank.Core.Application.Abstractions.Command;

namespace Frank.TestUtilities.InvalidServices;

public sealed record FakeCommand(string Value) : ICommand<FakeResponse>;

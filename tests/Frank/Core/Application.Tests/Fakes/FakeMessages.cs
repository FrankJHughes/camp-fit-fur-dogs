using Frank.Core.Application.Abstractions.Command;
using Frank.Core.Application.Abstractions.Query;

namespace Frank.Core.Application.Tests.Fakes;

public sealed record FakeResponse(string Value);

public sealed record FakeCommand(string Name) : ICommand<FakeResponse>;

public sealed record FakeQuery(string Filter) : IQuery<FakeResponse>;

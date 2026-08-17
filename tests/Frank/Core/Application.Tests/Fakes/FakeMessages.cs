using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.Core.Application.Tests.Fakes;

public sealed record FakeResponse(string Value);

public sealed record FakeCommand(string Name) : ICommand<FakeResponse>;

public sealed record FakeQuery(string Filter) : IQuery<FakeResponse>;

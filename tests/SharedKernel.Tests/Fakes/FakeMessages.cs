using SharedKernel.Abstractions;

namespace SharedKernel.Tests.Fakes;

public sealed record FakeResponse(string Value);

public sealed record FakeCommand(string Name) : ICommand<FakeResponse>;

public sealed record FakeQuery(string Filter) : IQuery<FakeResponse>;

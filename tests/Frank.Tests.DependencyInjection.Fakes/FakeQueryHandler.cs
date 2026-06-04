using Frank.Abstractions;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class FakeQueryHandler
    : IQueryHandler<FakeQuery, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeQuery query, CancellationToken ct)
        => Task.FromResult(new FakeResponse());
}

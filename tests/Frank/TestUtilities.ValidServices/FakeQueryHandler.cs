using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.TestUtilities.ValidServices;

public sealed class FakeQueryHandler
    : IQueryHandler<FakeQuery, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeQuery query, CancellationToken ct)
        => Task.FromResult(new FakeResponse());
}

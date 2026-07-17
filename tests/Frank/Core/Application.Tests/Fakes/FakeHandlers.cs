using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.Core.Application.Tests.Fakes;

public sealed class FakeCommandHandler : ICommandHandler<FakeCommand, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeCommand command, CancellationToken ct) =>
        Task.FromResult(new FakeResponse($"Handled:{command.Name}"));
}

public sealed class FakeQueryHandler : IQueryHandler<FakeQuery, FakeResponse>
{
    public Task<FakeResponse> HandleAsync(FakeQuery query, CancellationToken ct) =>
        Task.FromResult(new FakeResponse($"Queried:{query.Filter}"));
}

// public sealed class FakeDuplicateHandler : ICommandHandler<FakeCommand, FakeResponse>
// {
//     public Task<FakeResponse> HandleAsync(FakeCommand command, CancellationToken ct) =>
//         Task.FromResult(new FakeResponse("Duplicate"));
// }

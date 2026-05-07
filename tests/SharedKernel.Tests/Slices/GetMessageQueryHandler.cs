
namespace SharedKernel.Tests.Slices;

public sealed class GetMessageQueryHandler
    : IQueryHandler<GetMessageQuery, GetMessageResponse>
{
    public Task<GetMessageResponse> HandleAsync(GetMessageQuery query, CancellationToken ct)
        => Task.FromResult(new GetMessageResponse($"Message #{query.Id}"));
}


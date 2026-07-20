using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Domain.Sessions.Errors;

namespace Frank.Identity.Application.Sessions.GetSession;

public sealed class GetSessionByIdHandler(IGetSessionReader reader)
    : IQueryHandler<GetSessionQuery, GetSessionResponse?>
{
    public async Task<GetSessionResponse?> HandleAsync(
        GetSessionQuery query, CancellationToken ct)
    {

        var result = await reader.ReadAsync(query.TokenHash, ct);

        if (result is null)
        {
            throw new SessionNotFoundException();
        }
        return result;
    }
}

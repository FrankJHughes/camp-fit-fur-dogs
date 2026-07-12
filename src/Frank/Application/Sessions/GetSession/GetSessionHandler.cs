using Frank.Abstractions.Query;
using Frank.Application.Abstractions.Sessions.GetSession;
using Frank.Domain.Sessions.Errors;

namespace Frank.Application.Sessions.GetSession;

public sealed class GetSessionByIdHandler(IGetSessionReader reader)
    : IQueryHandler<GetSessionQuery, GetSessionResponse?>
{
    public async Task<GetSessionResponse?> HandleAsync(
        GetSessionQuery query, CancellationToken ct)
    {

        var result = await reader.GetSessionAsync(query.TokenHash, ct);

        if (result is null)
        {
            throw new SessionNotFoundException();
        }
        return result;
    }
}

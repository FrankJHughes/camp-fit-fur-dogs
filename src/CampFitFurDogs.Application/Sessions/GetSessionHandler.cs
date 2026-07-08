using CampFitFurDogs.Application.Abstractions.Sessions.GetSession;
using CampFitFurDogs.Domain.Sessions.Errors;
using Frank.Abstractions.Query;

namespace CampFitFurDogs.Application.Sessions.GetSession;

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

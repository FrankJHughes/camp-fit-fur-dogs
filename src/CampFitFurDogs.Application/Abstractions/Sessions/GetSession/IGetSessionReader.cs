
namespace CampFitFurDogs.Application.Abstractions.Sessions.GetSession;

public interface IGetSessionReader
{
    Task<GetSessionResponse?> GetSessionAsync(
        string tokenHash, CancellationToken ct);
}

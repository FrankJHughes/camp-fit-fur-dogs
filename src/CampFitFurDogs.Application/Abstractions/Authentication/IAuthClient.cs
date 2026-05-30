namespace CampFitFurDogs.Application.Abstractions.Authentication;

public interface IAuthClient
{
    Task<AuthToken> ExchangeAsync(
        string code,
        CancellationToken ct);

    Task<AuthUser> GetUserAsync(
        AuthToken accessToken,
        CancellationToken ct);
}

namespace CampFitFurDogs.Application.Abstractions.Authentication.Auth0;

public interface IAuth0Client
{
    Task<string> ExchangeCodeForTokenAsync(
        string code,
        string clientId,
        string clientSecret,
        string callbackUrl,
        string domain,
        CancellationToken ct);

    Task<Auth0UserInfo> GetUserInfoAsync(
        string accessToken,
        string domain,
        CancellationToken ct);
}

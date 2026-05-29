using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Auth0;
using CampFitFurDogs.Application.Abstractions.Identity.External;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Domain.Errors;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Application.Authentication;

public class AuthCallbackService : IAuthCallbackService
{
    private readonly IConfiguration _config;
    private readonly IAuth0Client _auth0;
    private readonly IExternalIdentityResolver _resolver;
    private readonly IAuditLogger _audit;

    public AuthCallbackService(
        IConfiguration config,
        IAuth0Client auth0,
        IExternalIdentityResolver resolver,
        IAuditLogger audit)
    {
        _config = config;
        _auth0 = auth0;
        _resolver = resolver;
        _audit = audit;
    }

    public async Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        var cfg = LoadConfig();

        // 1. Exchange authorization code for access token
        var accessToken = await _auth0.ExchangeCodeForTokenAsync(
            code,
            cfg.ClientId,
            cfg.ClientSecret,
            cfg.CallbackUrl,
            cfg.Domain,
            ct);

        // 2. Fetch user profile from Auth0
        var userInfo = await _auth0.GetUserInfoAsync(
            accessToken,
            cfg.Domain,
            ct);

        // 3. Resolve or create internal customer identity
        var customerId = await _resolver.ResolveAsync(
            userInfo.ExternalId,
            userInfo.FirstName,
            userInfo.LastName,
            userInfo.Email,
            ct);

        // 4. Audit success
        await _audit.LoginSucceeded(customerId, userInfo.ExternalId);

        // 5. Return result to API layer
        return new AuthCallbackResult(customerId, cfg.PostLoginRedirectUrl);
    }

    private Auth0Config LoadConfig()
    {
        var domain = _config["Auth0:Domain"];
        var clientId = _config["Auth0:ClientId"];
        var clientSecret = _config["Auth0:ClientSecret"];
        var callbackUrl = _config["Auth0:CallbackUrl"];
        var redirectUrl = _config["Auth0:PostLoginRedirectUrl"];

        if (string.IsNullOrWhiteSpace(domain) ||
            string.IsNullOrWhiteSpace(clientId) ||
            string.IsNullOrWhiteSpace(clientSecret) ||
            string.IsNullOrWhiteSpace(callbackUrl) ||
            string.IsNullOrWhiteSpace(redirectUrl))
        {
            throw new BadConfigurationException("Auth0 configuration is incomplete.");
        }

        return new Auth0Config(domain, clientId, clientSecret, callbackUrl, redirectUrl);
    }
}

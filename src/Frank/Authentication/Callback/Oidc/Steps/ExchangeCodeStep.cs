using System.Text.Json;
using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Settings;

namespace Frank.Authentication.Callback.Oidc.Steps;

public sealed class ExchangeCodeStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    private readonly HttpClient _http;
    private readonly AuthCallbackOidcSettings _options;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.exchange-code", "Exchange Authorization Code");

    public ExchangeCodeStep(HttpClient http, AuthCallbackOidcSettings options)
    {
        _http = http;
        _options = options;
    }

    public bool CanExecute(OidcAuthCallbackContext ctx)
        => ctx.Code is not null;

    public async Task<OidcAuthCallbackContext> ExecuteAsync(
        OidcAuthCallbackContext ctx,
        CancellationToken ct)
    {
        var tokenEndpoint = $"https://{_options.Authority}/oauth/token";

        var payload = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = ctx.Code,
            ["redirect_uri"] = _options.CallbackUrl
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(payload)
        };

        var response = await _http.SendAsync(req, ct);

        if (!response.IsSuccessStatusCode)
            throw new OidcProtocolException("Failed to exchange authorization code for tokens.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        var accessToken = root.TryGetProperty("access_token", out var at)
            ? at.GetString()
            : null;

        var idToken = root.TryGetProperty("id_token", out var id)
            ? id.GetString()
            : null;

        return ctx with
        {
            AccessToken = accessToken,
            IdToken = idToken
        };
    }
}

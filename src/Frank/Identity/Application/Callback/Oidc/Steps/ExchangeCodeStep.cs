using System.Text.Json;
using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.Options;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

public sealed class ExchangeCodeStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    private readonly HttpClient _http;
    private readonly OidcCallbackSettings _options;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.exchange-code", "Exchange Authorization Code");

    public ExchangeCodeStep(HttpClient http, IOptionsMonitor<OidcCallbackSettings> options)
    {
        _http = http;
        _options = options.CurrentValue;
    }

    public bool CanExecute(OidcCallbackContext ctx)
        => ctx.Code is not null;

    public async Task<OidcCallbackContext> ExecuteAsync(
        OidcCallbackContext ctx,
        CancellationToken ct)
    {
        var tokenEndpoint = $"{_options.Authority}/oauth/token";

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

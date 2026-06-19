using System.Net;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc;

public sealed class FakeOidcHttpClient : HttpMessageHandler
{
    public string? TokenResponseJson { get; set; }
    public string? JwksResponseJson { get; set; }

    public bool FailTokenEndpoint { get; set; }
    public bool FailJwksEndpoint { get; set; }

    public HttpClient CreateClient()
        => new(this);

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = request.RequestUri!.AbsoluteUri;

        // Simulate token endpoint
        if (url.Contains("/oauth/token"))
        {
            if (FailTokenEndpoint)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var json = TokenResponseJson ?? """{ "access_token": "fake-access", "id_token": "fake-id" }""";

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });
        }

        // Simulate JWKS endpoint
        if (url.Contains("/.well-known/jwks.json"))
        {
            if (FailJwksEndpoint)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var json = JwksResponseJson ?? """{ "keys": [] }""";

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });
        }

        throw new InvalidOperationException($"Unexpected URL: {url}");
    }
}

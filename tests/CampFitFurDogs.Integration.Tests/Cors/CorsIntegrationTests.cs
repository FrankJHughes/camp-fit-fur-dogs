using System.Net;
using CampFitFurDogs.TestUtilities.Fixtures;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;

namespace CampFitFurDogs.Integration.Tests.Cors;

[Collection("API Collection")]
public class CorsIntegrationTests
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;

    private const string AllowedOrigin = "https://camp-fit-fur-dogs.vercel.app";
    private const string DeniedOrigin = "https://evil.example.com";

    public CorsIntegrationTests(ApiFactoryFixture factoryFixture, PostgresFixture postgresFixture)
    {
        _factory = factoryFixture.Factory;
        _factory.UseContainer(postgresFixture.Container);

        // Sets Frontend__BaseUrl → used by CorsStartupModule
        _factory.WithFrontendBaseUrl(AllowedOrigin);

        _client = _factory.CreateClient();
    }

    // ---------------------------------------------------------------------
    // 1. ALLOWED ORIGIN
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Allowed_origin_receives_cors_headers()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/health");
        request.Headers.Add("Origin", AllowedOrigin);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue();

        origins!.Single().Should().Be(AllowedOrigin);

        response.Headers.TryGetValues("Access-Control-Allow-Credentials", out var creds)
            .Should().BeTrue();

        creds!.Single().Should().Be("true");
    }

    // ---------------------------------------------------------------------
    // 2. DENIED ORIGIN
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Denied_origin_does_not_receive_cors_headers()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/health");
        request.Headers.Add("Origin", DeniedOrigin);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
        response.Headers.Contains("Access-Control-Allow-Credentials").Should().BeFalse();
    }

    // ---------------------------------------------------------------------
    // 3. PREFLIGHT SUCCESS
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Preflight_request_returns_expected_cors_headers()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/health");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "Authorization, Content-Type");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.OK);

        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue();

        origins!.Single().Should().Be(AllowedOrigin);

        response.Headers.TryGetValues("Access-Control-Allow-Methods", out var methods)
            .Should().BeTrue();

        methods!.Single().Should().Contain("GET");

        response.Headers.TryGetValues("Access-Control-Allow-Headers", out var headers)
            .Should().BeTrue();

        headers!.Single().Should().Contain("Authorization")
            .And.Contain("Content-Type");

        response.Headers.TryGetValues("Access-Control-Max-Age", out var maxAge)
            .Should().BeTrue("CorsStartupModule sets preflight caching");
    }

    // ---------------------------------------------------------------------
    // 4. PREFLIGHT FAILURE: MISSING METHOD
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Preflight_missing_access_control_request_method_is_rejected()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/health");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Headers", "Authorization");

        var response = await _client.SendAsync(request);

        // ASP.NET Core returns 204 or 405 depending on routing
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.MethodNotAllowed);

        // Origin is still echoed because origin is allowed
        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue();

        origins!.Single().Should().Be(AllowedOrigin);

        // But no method/header approval
        response.Headers.Contains("Access-Control-Allow-Methods").Should().BeFalse();
        response.Headers.Contains("Access-Control-Allow-Headers").Should().BeFalse();
    }

    // ---------------------------------------------------------------------
    // 5. PREFLIGHT FAILURE: DISALLOWED METHOD
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Preflight_disallowed_method_is_rejected()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/health");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "PATCH");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue();

        origins!.Single().Should().Be(AllowedOrigin);

        response.Headers.TryGetValues("Access-Control-Allow-Methods", out var methods)
            .Should().BeTrue();

        methods!.Single().Should().NotContain("PATCH");
    }

    // ---------------------------------------------------------------------
    // 6. PREFLIGHT FAILURE: DISALLOWED HEADER
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Preflight_disallowed_header_is_rejected()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/health");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "X-Custom");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue();

        origins!.Single().Should().Be(AllowedOrigin);

        response.Headers.TryGetValues("Access-Control-Allow-Headers", out var headers)
            .Should().BeTrue();

        headers!.Single().Should().Contain("Authorization")
            .And.Contain("Content-Type")
            .And.NotContain("X-Custom");
    }

    // ---------------------------------------------------------------------
    // 7. NO WILDCARD REGRESSION
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Cors_headers_should_not_contain_wildcards()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/health");
        request.Headers.Add("Origin", AllowedOrigin);

        var response = await _client.SendAsync(request);

        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue();

        origins!.Single().Should().NotContain("*");
    }

    // ---------------------------------------------------------------------
    // 8. CORS APPLIES BEFORE AUTH
    // ---------------------------------------------------------------------

    [Fact(Skip = "Authentication not implemented yet — no endpoint returns 401")]
    public async Task Cors_headers_are_present_even_when_auth_fails()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
        request.Headers.Add("Origin", AllowedOrigin);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins)
            .Should().BeTrue("CORS must run before authentication");

        origins!.Single().Should().Be(AllowedOrigin);
    }

}

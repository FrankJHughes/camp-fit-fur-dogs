using System.Net;
using System.Net.Http;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Infrastructure.Identity.Oidc;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CampFitFurDogs.TestUtilities.Builders;

public class TestClientBuilder
{
    private readonly CampFitFurDogsApiFactory _factory;

    private Action<IConfigurationBuilder>? _configOverrides;
    private Dictionary<string, HttpResponseMessage>? _fakeOidcResponses;
    private IIdentityResolver? _identityResolver;
    private IAuditLogger? _auditLogger;
    private IAuthCallbackService? _authCallbackService;
    private Dictionary<string, string>? _fakeClaims;

    public TestClientBuilder(CampFitFurDogsApiFactory factory)
    {
        _factory = factory;
    }

    public TestClientBuilder WithConfigOverrides(Action<IConfigurationBuilder> cfg)
    {
        _configOverrides = cfg;
        return this;
    }

    public TestClientBuilder WithFakeOidcResponses(Dictionary<string, HttpResponseMessage> responses)
    {
        _fakeOidcResponses = responses;
        return this;
    }

    public TestClientBuilder WithIdentityResolver(IIdentityResolver resolver)
    {
        _identityResolver = resolver;
        return this;
    }

    public TestClientBuilder WithAuditLogger(IAuditLogger logger)
    {
        _auditLogger = logger;
        return this;
    }

    public TestClientBuilder WithAuthCallbackService(IAuthCallbackService service)
    {
        _authCallbackService = service;
        return this;
    }

    public TestClientBuilder WithFakeClaims(Dictionary<string, string> claims)
    {
        _fakeClaims = claims;
        return this;
    }

    public HttpClient BuildClient()
    {
        var workingFactory = _factory;

        if (_configOverrides is not null)
            workingFactory = workingFactory.WithConfigOverrides(_configOverrides);

        var cookieContainer = new CookieContainer();

        var configuredFactory = workingFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                //
                // Replace IAuthClient
                //
                services.RemoveAll<IAuthClient>();

                if (_fakeOidcResponses is not null)
                {
                    services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() =>
                            new FakeHttpMessageHandler(_fakeOidcResponses));
                }
                else
                {
                    var guid = Guid.NewGuid();
                    services.AddSingleton<IAuthClient>(new FakeAuthClient
                    {
                        TokenToReturn = new AuthToken("fake-token"),
                        UserToReturn = new AuthUser(
                            ExternalId: $"test-auth0|{guid}",
                            GivenName: "Test",
                            FamilyName: "User",
                            Email: $"test-{guid}@example.com"
                        )
                    });
                }

                //
                // Identity resolver override
                //
                if (_identityResolver is not null)
                    services.AddSingleton<IIdentityResolver>(_identityResolver);

                //
                // Audit logger override
                //
                if (_auditLogger is not null)
                    services.AddSingleton<IAuditLogger>(_auditLogger);

                //
                // Auth callback override
                //
                if (_authCallbackService is not null)
                {
                    services.RemoveAll<IAuthCallbackService>();
                    services.AddSingleton<IAuthCallbackService>(_authCallbackService);
                }

                //
                // FakeAuthHandler claim overrides
                // Only apply if FakeAuthHandler is active
                //
                if (_fakeClaims is not null && !_factory.UsesCookieAuthOnly)
                {
                    services.RemoveAll<FakeAuthHandlerClaims>();
                    services.AddSingleton(new FakeAuthHandlerClaims(_fakeClaims));
                }
            });

            //
            // Attach CookieContainer for CookieAuth mode
            //
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<HttpMessageHandler>(_ =>
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        UseCookies = true,
                        CookieContainer = cookieContainer
                    });
            });
        });

        var scheme = _factory.UsesCookieAuthOnly ? "https" : "http";

        var client = configuredFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
            BaseAddress = new Uri($"{scheme}://localhost")
        });

        return client;
    }
}

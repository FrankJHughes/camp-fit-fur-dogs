using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Infrastructure.Identity.Oidc;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CampFitFurDogs.TestUtilities.Builders;

public class TestClientBuilder
{
    private readonly TestWebApplicationFactory _factory;

    private Action<IConfigurationBuilder>? _configOverrides;
    private Dictionary<string, HttpResponseMessage>? _fakeOidcResponses;
    private IIdentityResolver? _identityResolver;
    private IAuditLogger? _auditLogger;
    private IAuthCallbackService? _authCallbackService;
    private string? _environment;

    public TestClientBuilder(TestWebApplicationFactory factory)
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

    public TestClientBuilder WithEnvironment(string env)
    {
        _environment = env;
        return this;
    }

    public TestClientBuilder WithAuthCallbackService(IAuthCallbackService service)
    {
        _authCallbackService = service;
        return this;
    }

    public HttpClient BuildClient()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            if (_environment is not null)
                builder.UseEnvironment(_environment);

            if (_configOverrides is not null)
            {
                builder.ConfigureAppConfiguration((_, cfg) =>
                {
                    _configOverrides(cfg);
                });
            }

            builder.ConfigureServices(services =>
            {
                // Fake OIDC HTTP responses
                if (_fakeOidcResponses is not null)
                {
                    services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() =>
                            new FakeHttpMessageHandler(_fakeOidcResponses));
                }

                if (_identityResolver is not null)
                    services.AddSingleton<IIdentityResolver>(_identityResolver);

                if (_auditLogger is not null)
                    services.AddSingleton<IAuditLogger>(_auditLogger);

                // ------------------------------------------------------------
                // OVERRIDE AUTH CALLBACK SERVICE FOR TESTS
                // ------------------------------------------------------------
                if (_authCallbackService is not null)
                {
                    services.RemoveAll<IAuthCallbackService>();
                    services.AddSingleton<IAuthCallbackService>(_authCallbackService);
                }
            });
        });

        return factory.CreateClient(new()
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }
}

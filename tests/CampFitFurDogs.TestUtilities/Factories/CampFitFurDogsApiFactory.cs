using System.Net;
using System.Security.Claims;
using CampFitFurDogs.Api.Horizontals.Startup;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Fakes;
using CampFitFurDogs.TestUtilities.Infrastructure;
using Frank.Abstractions;
using Frank.Api.Startup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.TestUtilities.Factories;

public sealed class CampFitFurDogsApiFactory : WebApplicationFactory<Program>
{
    private readonly List<Action<IConfigurationBuilder>> _configOverrides = new();
    private string? _forcedEnvironment;
    private bool _disableDatabase;
    private bool _useCookieAuthOnly;
    private bool _overrideCookiesForHttp = true;

    public PostgreSqlContainer? Postgres { get; private set; }

    public bool UsesCookieAuthOnly => _useCookieAuthOnly;

    // ------------------------------------------------------------
    // CLONE SUPPORT
    // ------------------------------------------------------------
    public CampFitFurDogsApiFactory Clone()
    {
        var clone = new CampFitFurDogsApiFactory
        {
            _forcedEnvironment = _forcedEnvironment,
            _disableDatabase = _disableDatabase,
            _useCookieAuthOnly = _useCookieAuthOnly,
            _overrideCookiesForHttp = _overrideCookiesForHttp,
            Postgres = Postgres
        };

        foreach (var o in _configOverrides)
            clone._configOverrides.Add(o);

        return clone;
    }

    public HttpClient CreateClientWithCookies()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            AllowAutoRedirect = false
        });
    }

    // ------------------------------------------------------------
    // MODES
    // ------------------------------------------------------------
    public CampFitFurDogsApiFactory WithCookieAuthOnly()
    {
        _useCookieAuthOnly = true;
        return this;
    }

    public CampFitFurDogsApiFactory WithoutCookieHttpOverride()
    {
        _overrideCookiesForHttp = false;
        return this;
    }

    public CampFitFurDogsApiFactory UseContainer(PostgreSqlContainer container)
    {
        Postgres = container;
        _disableDatabase = false;
        return this;
    }

    public CampFitFurDogsApiFactory WithoutDatabase()
    {
        _disableDatabase = true;
        return this;
    }

    internal void InternalConfigOverrides(Action<IConfigurationBuilder> configure)
        => _configOverrides.Add(configure);

    internal CampFitFurDogsApiFactory SetEnvironment(string environment)
    {
        _forcedEnvironment = environment;
        return this;
    }

    // ------------------------------------------------------------
    // HOST CREATION
    // ------------------------------------------------------------
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var environment = _forcedEnvironment ?? "Testing";
        var overrides = _configOverrides.ToList();

        builder.UseEnvironment(environment);

        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = environment,
                ["Frontend__BaseUrl"] = "http://localhost:5173",
                ["Authentication:Oidc:Authority"] = "test.example.com",
                ["Authentication:Oidc:ClientId"] = "test-client-id",
                ["Authentication:Oidc:ClientSecret"] = "test-secret",
                ["Authentication:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
                ["Authentication:Oidc:PostLoginRedirectUrl"] = "http://localhost:5173/",
                ["Authentication:Oidc:Disabled"] = "true"
            });

            foreach (var apply in overrides)
                apply(cfg);
        });

        return base.CreateHost(builder);
    }

    // ------------------------------------------------------------
    // WEB HOST CONFIGURATION
    // ------------------------------------------------------------
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            //
            // COOKIE OVERRIDES
            //
            if (_overrideCookiesForHttp)
            {
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    opts.Cookie.SameSite = SameSiteMode.Lax;
                    opts.Cookie.Domain = null;
                });
            }

            //
            // DEFAULT FAKE OIDC CLIENT
            //
            services.RemoveAll<IAuthClient>();
            services.AddSingleton<IAuthClient, FakeOidcAuthClient>();

            services.AddHttpContextAccessor();

            //
            // COOKIE-AUTH-ONLY MODE
            //
            if (_useCookieAuthOnly)
            {
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.HttpOnly = true;
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    opts.Cookie.SameSite = SameSiteMode.Lax;
                });
            }

            //
            // DATABASE MODE
            //
            if (!_disableDatabase)
            {
                if (Postgres is null)
                    throw new InvalidOperationException("Postgres container not initialized.");

                services.RemoveAll<DbContextOptions<AppDbContext>>();

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(Postgres.GetConnectionString()));

                services.AddHostedService<TestDatabaseInitializer>();
            }
            else
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
            }

            //
            // TEST-ONLY ENDPOINTS (SAFE VERSION)
            //
            services.AddTransient<IStartupFilter, TestEndpointsStartupFilter>();
        });
    }

    // ------------------------------------------------------------
    // TEST ENDPOINTS INJECTED SAFELY VIA STARTUP FILTER
    // ------------------------------------------------------------
    private sealed class TestEndpointsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                next(app); // run the real pipeline first

                app.UseEndpoints(endpoints =>
                {
                    // POST /__test__/sign-in
                    endpoints.MapPost("/__test__/sign-in", async context =>
                    {
                        var payload = await context.Request.ReadFromJsonAsync<SignInPayload>();
                        var sub = payload?.Sub;

                        if (string.IsNullOrWhiteSpace(sub))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }

                        var claims = new[] { new Claim("sub", sub) };
                        var identity = new ClaimsIdentity(claims, "Test");
                        var principal = new ClaimsPrincipal(identity);

                        await context.SignInAsync("cfd.session", principal);

                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    });

                    // GET /__test__/current-user-id
                    endpoints.MapGet("/__test__/current-user-id", async context =>
                    {
                        var sub = context.User.FindFirstValue("sub") ?? string.Empty;
                        await context.Response.WriteAsync(sub);
                    });
                });
            };
        }
    }

    private sealed record SignInPayload(string Sub);
}

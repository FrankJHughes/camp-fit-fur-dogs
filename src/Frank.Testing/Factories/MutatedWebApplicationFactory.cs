using System.Net.Http.Json;
using Frank.Api.Endpoints;
using Frank.Testing.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Frank.Testing.Factories;

public abstract class MutatedWebApplicationFactory<TEntryPoint, TContext, TClientContext>
    : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
    where TContext : MutatedWebApplicationContext<TContext>
    where TClientContext : MutatedWebApplicationClientContext<TClientContext>
{
    private TContext _ctx;
    private PostgreSqlContainer? _db;

    protected MutatedWebApplicationFactory(TContext ctx)
    {
        _ctx = ctx;
    }

    // ------------------------------------------------------------
    // CLIENT CREATION
    // ------------------------------------------------------------
    public HttpClient CreateClient(TClientContext clientCtx)
    {
        var client = base.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            AllowAutoRedirect = false
        });

        foreach (var kvp in clientCtx.DefaultHeaders)
            client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);

        if (clientCtx.AuthenticatedUserSub is string id)
        {
            var payload = new
            {
                Sub = id,
                Scheme = clientCtx.SignInScheme
            };

            var response = client.PostAsJsonAsync("/__test__/sign-in", payload).Result;
            response.EnsureSuccessStatusCode();
        }

        return client;
    }

    // ------------------------------------------------------------
    // CONFIGURATION + SERVICES
    // ------------------------------------------------------------
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Environment must be set here, not in CreateHost, to avoid breaking host resolution.
        builder.UseEnvironment(_ctx.Environment);

        builder.ConfigureAppConfiguration((context, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = _ctx.Environment
            });

            foreach (var apply in _ctx.ConfigOverrides)
                apply(cfg);
        });

        builder.ConfigureServices((context, services) =>
        {
            // ------------------------------------------------------------
            // COOKIE AUTH ADJUSTMENTS FOR TESTING
            // ------------------------------------------------------------
            services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
            {
                opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
                opts.Cookie.SameSite = SameSiteMode.Lax;
            });

            if (_ctx.UseCookieAuthOnly)
            {
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.HttpOnly = true;
                });
            }

            if (!_ctx.OverrideCookiesForHttp)
            {
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            }

            foreach (var apply in _ctx.CookieOptionsOverrides)
                services.PostConfigureAll(apply);

            // ------------------------------------------------------------
            // SERVICE OVERRIDES
            // ------------------------------------------------------------
            foreach (var apply in _ctx.ServiceOverrides)
                apply(services);

            // ------------------------------------------------------------
            // FAKE SERVICE REGISTRATIONS
            // ------------------------------------------------------------
            foreach (var kvp in _ctx.Fakes)
            {
                services.RemoveAll(kvp.Key);
                services.AddSingleton(kvp.Key, kvp.Value);
            }

            // ------------------------------------------------------------
            // DATABASE HANDLING
            // ------------------------------------------------------------
            if (!_ctx.DisableDatabase && _ctx.Postgres is not null)
            {
                ConfigureDatabase(context, services, _ctx.Postgres);
            }
            else
            {
                ConfigureDatabaseDisabled(context, services);
            }

            // ------------------------------------------------------------
            // ENDPOINT DISCOVERY (FROM CONTEXT)
            // ------------------------------------------------------------
            foreach (var asm in _ctx.EndpointAssemblies)
                EndpointDiscovery.AddEndpoints(asm);

            // ------------------------------------------------------------
            // ALLOW SUBCLASSES TO MUTATE SERVICES
            // ------------------------------------------------------------
            ConfigureMutations(context, services);
        });
    }

    // ------------------------------------------------------------
    // EXTENSION POINTS
    // ------------------------------------------------------------
    protected virtual void ConfigureMutations(
        WebHostBuilderContext context,
        IServiceCollection services)
    {
    }

    protected virtual void ConfigureDatabase(
        WebHostBuilderContext context,
        IServiceCollection services,
        PostgreSqlContainer postgres)
    {
    }

    protected virtual void ConfigureDatabaseDisabled(
        WebHostBuilderContext context,
        IServiceCollection services)
    {
    }

    // ------------------------------------------------------------
    // DATABASE LIFECYCLE
    // ------------------------------------------------------------
    public async Task<MutatedWebApplicationFactory<TEntryPoint, TContext, TClientContext>> WithDatabaseAsync(
        Func<PostgreSqlBuilder, PostgreSqlBuilder>? configureBuilder = null)
    {
        var builder = new PostgreSqlBuilder("postgres:17")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithDatabase("testdb");

        if (configureBuilder is not null)
            builder = configureBuilder(builder);

        _db = builder.Build();
        await _db.StartAsync();

        _ctx = _ctx.WithDatabase(true, _db);

        return this;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _db?.DisposeAsync().AsTask().Wait();
    }
}

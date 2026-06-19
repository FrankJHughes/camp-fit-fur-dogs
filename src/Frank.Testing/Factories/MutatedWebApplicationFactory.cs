using System.Net.Http;
using System.Net.Http.Json;
using Frank.Api;
using Frank.Testing.Contexts;
using Frank.Testing.Endpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                Scheme = clientCtx.SignInScheme // <- API-specific, provided by consumer
            };

            var response = client.PostAsJsonAsync("/__test__/sign-in", payload).Result;
            response.EnsureSuccessStatusCode();
        }

        return client;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_ctx.Environment);

        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = _ctx.Environment
            });

            foreach (var apply in _ctx.ConfigOverrides)
                apply(cfg);
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
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

            foreach (var apply in _ctx.ServiceOverrides)
                apply(services);

            if (!_ctx.DisableDatabase && _ctx.Postgres is not null)
            {
                ConfigureDatabase(context, services, _ctx.Postgres);
            }
            else
            {
                ConfigureDatabaseDisabled(context, services);
            }

            var assembly = typeof(SignInEndpoint).Assembly;
            EndpointDiscovery.AddEndpoints(assembly);

            ConfigureMutations(context, services);
        });
    }

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

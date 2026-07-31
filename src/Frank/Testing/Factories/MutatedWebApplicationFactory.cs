using Frank.Core.Api.Endpoints;
using Frank.Testing.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
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
    public IServiceCollection? ServiceCollection { get; private set; }

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

        ApplyAuthenticationAsync(client, clientCtx).GetAwaiter().GetResult();

        return client;
    }

    // ------------------------------------------------------------
    // AUTHENTICATION EXTENSION POINT
    // ------------------------------------------------------------
    protected virtual Task ApplyAuthenticationAsync(HttpClient client, TClientContext clientCtx)
    {
        return Task.CompletedTask;
    }

    // ------------------------------------------------------------
    // CONFIGURATION + SERVICES
    // ------------------------------------------------------------
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_ctx.Environment);

        builder.ConfigureAppConfiguration((context, cfg) =>
        {
            foreach (var apply in _ctx.ConfigOverrides)
                apply(cfg);
        });

        builder.ConfigureServices((context, services) =>
        {
            ServiceCollection = services;

            foreach (var apply in _ctx.ServiceOverrides)
                apply(services);

            foreach (var kvp in _ctx.Fakes)
            {
                services.RemoveAll(kvp.Key);
                services.AddSingleton(kvp.Key, kvp.Value);
            }

            if (!_ctx.DisableDatabase && _ctx.Postgres is not null)
            {
                ConfigureDatabase(context, services, _ctx.Postgres);
            }
            else
            {
                ConfigureDatabaseDisabled(context, services);
            }

            if (_ctx.EndpointAssemblies.Count > 0)
            {
                services.AddFrankCoreApiEndpoints(_ctx.EndpointAssemblies);
            }

            ConfigureMutations(context, services);

            // ------------------------------------------------------------
            // COOKIE DOWNGRADE VIA STARTUP FILTER
            // ------------------------------------------------------------
            if (_ctx.OverrideCookiesForHttp)
            {
                services.AddSingleton<IStartupFilter>(new CookieRewriteStartupFilter());
            }

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

// ------------------------------------------------------------
// STARTUP FILTER FOR COOKIE REWRITE
// ------------------------------------------------------------
public sealed class CookieRewriteStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Use(nextDelegate => async context =>
            {
                context.Response.OnStarting(() =>
                {
                    if (context.Response.Headers.TryGetValue("Set-Cookie", out var setCookieHeaders))
                    {
                        var rewritten = new List<string>();

                        foreach (var header in setCookieHeaders)
                        {
                            if (header is null)
                            {
                                continue;
                            }

                            var modified = header
                                .Replace("Secure;", "", StringComparison.OrdinalIgnoreCase)
                                .Replace("Secure", "", StringComparison.OrdinalIgnoreCase);

                            rewritten.Add(modified);
                        }

                        context.Response.Headers["Set-Cookie"] = rewritten.ToArray();
                    }

                    return Task.CompletedTask;
                });

                await nextDelegate(context);
            });

            next(app);
        };
    }
}


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

/// <summary>
/// A highly configurable <see cref="WebApplicationFactory{TEntryPoint}"/> that
/// integrates with the mutation‑based testing contexts used in the Frank testing
/// harness.
/// <para>
/// This factory allows test suites to dynamically shape the test host using:
/// </para>
/// <list type="bullet">
/// <item><description>Environment overrides</description></item>
/// <item><description>Configuration overrides</description></item>
/// <item><description>Service overrides</description></item>
/// <item><description>Cookie authentication downgrades</description></item>
/// <item><description>Endpoint assembly discovery</description></item>
/// <item><description>Fake service injection</description></item>
/// <item><description>Optional PostgreSQL test containers</description></item>
/// </list>
/// <para>
/// The generic <typeparamref name="TContext"/> and <typeparamref name="TClientContext"/>
/// parameters ensure that both application‑level and client‑level contexts can be
/// mutated independently while remaining strongly typed.
/// </para>
/// </summary>
/// <typeparam name="TEntryPoint">The ASP.NET Core entry point type.</typeparam>
/// <typeparam name="TContext">The application context type.</typeparam>
/// <typeparam name="TClientContext">The client context type.</typeparam>
public abstract class MutatedWebApplicationFactory<TEntryPoint, TContext, TClientContext>
    : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
    where TContext : MutatedWebApplicationContext<TContext>
    where TClientContext : MutatedWebApplicationClientContext<TClientContext>
{
    private TContext _ctx;
    private PostgreSqlContainer? _db;

    /// <summary>
    /// The underlying service collection used during test host construction.
    /// Useful for introspection or advanced mutation scenarios.
    /// </summary>
    public IServiceCollection? ServiceCollection { get; private set; }

    /// <summary>
    /// Initializes a new instance of the factory using the provided mutated
    /// application context.
    /// </summary>
    /// <param name="ctx">The initial application context.</param>
    protected MutatedWebApplicationFactory(TContext ctx)
    {
        _ctx = ctx;
    }

    // ------------------------------------------------------------
    // CLIENT CREATION
    // ------------------------------------------------------------

    /// <summary>
    /// Creates an <see cref="HttpClient"/> configured according to the provided
    /// client context, including default headers and optional authentication
    /// simulation.
    /// </summary>
    /// <param name="clientCtx">The client context used to mutate the client.</param>
    /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
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

    /// <summary>
    /// Allows derived factories to simulate authentication for the test client.
    /// <para>
    /// The default implementation does nothing, but subclasses may inject cookies,
    /// bearer tokens, or custom authentication flows.
    /// </para>
    /// </summary>
    protected virtual Task ApplyAuthenticationAsync(HttpClient client, TClientContext clientCtx)
    {
        return Task.CompletedTask;
    }

    // ------------------------------------------------------------
    // CONFIGURATION + SERVICES
    // ------------------------------------------------------------

    /// <summary>
    /// Configures the test web host using the mutated application context.
    /// <para>
    /// This includes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Environment selection</description></item>
    /// <item><description>Configuration overrides</description></item>
    /// <item><description>Service overrides</description></item>
    /// <item><description>Fake service injection</description></item>
    /// <item><description>Database configuration</description></item>
    /// <item><description>Endpoint assembly registration</description></item>
    /// <item><description>Cookie rewrite startup filter</description></item>
    /// </list>
    /// </summary>
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

            if (_ctx.OverrideCookiesForHttp)
            {
                services.AddSingleton<IStartupFilter>(new CookieRewriteStartupFilter());
            }
        });
    }

    // ------------------------------------------------------------
    // EXTENSION POINTS
    // ------------------------------------------------------------

    /// <summary>
    /// Allows derived factories to apply additional service mutations beyond
    /// configuration and DI overrides.
    /// </summary>
    protected virtual void ConfigureMutations(
        WebHostBuilderContext context,
        IServiceCollection services)
    {
    }

    /// <summary>
    /// Allows derived factories to configure database services when a PostgreSQL
    /// container is enabled.
    /// </summary>
    protected virtual void ConfigureDatabase(
        WebHostBuilderContext context,
        IServiceCollection services,
        PostgreSqlContainer postgres)
    {
    }

    /// <summary>
    /// Allows derived factories to configure services when database usage is disabled.
    /// </summary>
    protected virtual void ConfigureDatabaseDisabled(
        WebHostBuilderContext context,
        IServiceCollection services)
    {
    }

    // ------------------------------------------------------------
    // DATABASE LIFECYCLE
    // ------------------------------------------------------------

    /// <summary>
    /// Starts a PostgreSQL test container and mutates the application context to
    /// enable database usage.
    /// </summary>
    /// <param name="configureBuilder">
    /// Optional builder mutation for customizing the PostgreSQL container.
    /// </param>
    /// <returns>The same factory instance.</returns>
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

    /// <summary>
    /// Disposes the factory and any associated PostgreSQL container.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _db?.DisposeAsync().AsTask().Wait();
    }
}

// ------------------------------------------------------------
// STARTUP FILTER FOR COOKIE REWRITE
// ------------------------------------------------------------

/// <summary>
/// A startup filter that rewrites <c>Set-Cookie</c> headers to remove the
/// <c>Secure</c> attribute, enabling cookie usage over HTTP in test environments.
/// <para>
/// This is necessary because many authentication flows require cookies but
/// ASP.NET Core enforces <c>Secure</c> cookies by default.
/// </para>
/// </summary>
public sealed class CookieRewriteStartupFilter : IStartupFilter
{
    /// <summary>
    /// Configures the middleware pipeline to rewrite cookie headers before the
    /// response is sent.
    /// </summary>
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
                                continue;

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

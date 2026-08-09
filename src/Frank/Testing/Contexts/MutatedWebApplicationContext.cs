using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Frank.Testing.Contexts;

/// <summary>
/// Represents a mutable testing context for configuring and customizing a
/// web application test environment.
/// <para>
/// This abstraction is used by the Frank test harness to simulate different
/// hosting environments, authentication modes, configuration overrides,
/// service overrides, endpoint assemblies, and fake infrastructure.
/// </para>
/// <para>
/// The generic <typeparamref name="TSelf"/> pattern enables fluent,
/// self‑referencing mutation APIs in derived test contexts, ensuring that
/// each mutation produces a new immutable record instance.
/// </para>
/// </summary>
/// <typeparam name="TSelf">
/// The concrete context type inheriting from this record, enabling
/// self‑referencing fluent mutation patterns.
/// </typeparam>
public abstract record MutatedWebApplicationContext<TSelf>
    where TSelf : MutatedWebApplicationContext<TSelf>
{
    /// <summary>
    /// The hosting environment name used when building the test application.
    /// Defaults to <c>"Testing"</c>.
    /// </summary>
    public string Environment { get; init; } = "Testing";

    /// <summary>
    /// Indicates whether the test harness should disable database usage entirely.
    /// When <c>true</c>, no PostgreSQL container is started.
    /// </summary>
    public bool DisableDatabase { get; init; }

    /// <summary>
    /// The PostgreSQL test container instance used when database support is enabled.
    /// May be <c>null</c> when database usage is disabled.
    /// </summary>
    public PostgreSqlContainer? Postgres { get; init; }

    /// <summary>
    /// Indicates whether the test harness should use cookie authentication exclusively,
    /// bypassing other authentication schemes such as OIDC.
    /// </summary>
    public bool UseCookieAuthOnly { get; init; }

    /// <summary>
    /// Indicates whether cookies should be overridden for HTTP scenarios,
    /// typically used to ensure deterministic behavior in test environments.
    /// </summary>
    public bool OverrideCookiesForHttp { get; init; } = true;

    /// <summary>
    /// A list of configuration builder mutations applied before the test
    /// application is constructed.
    /// Useful for injecting test‑specific configuration values.
    /// </summary>
    public virtual IReadOnlyList<Action<IConfigurationBuilder>> ConfigOverrides { get; init; }
        = [];

    /// <summary>
    /// A list of service collection mutations applied during test application
    /// construction.
    /// Enables overriding or replacing DI registrations for test scenarios.
    /// </summary>
    public virtual IReadOnlyList<Action<IServiceCollection>> ServiceOverrides { get; init; }
        = [];

    /// <summary>
    /// A list of cookie authentication option mutations applied when configuring
    /// cookie authentication in the test harness.
    /// </summary>
    public virtual IReadOnlyList<Action<CookieAuthenticationOptions>> CookieOptionsOverrides { get; init; }
        = [];

    /// <summary>
    /// The set of assemblies containing API endpoints to be loaded by the test harness.
    /// Defaults to the Frank.Testing assembly.
    /// </summary>
    public virtual IReadOnlyList<Assembly> EndpointAssemblies { get; init; }
        = [typeof(Frank.Testing.AssemblyMarker).Assembly];

    // ------------------------------------------------------------
    // FLUENT MUTATION HELPERS
    // ------------------------------------------------------------

    /// <summary>
    /// Sets the hosting environment name for the test application.
    /// </summary>
    public TSelf WithEnvironment(string environment)
        => (TSelf)(this with { Environment = environment });

    /// <summary>
    /// Enables or disables database usage and optionally assigns a PostgreSQL container.
    /// </summary>
    public TSelf WithDatabase(bool enabled, PostgreSqlContainer? postgres)
        => (TSelf)(this with { DisableDatabase = !enabled, Postgres = postgres });

    /// <summary>
    /// Configures the test harness to use cookie authentication exclusively.
    /// </summary>
    public TSelf WithCookieAuthOnly(bool value = true)
        => (TSelf)(this with { UseCookieAuthOnly = value });

    /// <summary>
    /// Enables or disables cookie overrides for HTTP scenarios.
    /// </summary>
    public TSelf WithOverrideCookiesForHttp(bool value)
        => (TSelf)(this with { OverrideCookiesForHttp = value });

    /// <summary>
    /// Adds a configuration override to be applied during test application setup.
    /// </summary>
    public TSelf WithConfigOverride(Action<IConfigurationBuilder> apply)
        => (TSelf)(this with { ConfigOverrides = ConfigOverrides.Append(apply).ToList() });

    /// <summary>
    /// Adds a service override to be applied during test application DI setup.
    /// </summary>
    public TSelf WithServiceOverride(Action<IServiceCollection> apply)
        => (TSelf)(this with { ServiceOverrides = ServiceOverrides.Append(apply).ToList() });

    /// <summary>
    /// Adds a cookie authentication options override for customizing cookie behavior.
    /// </summary>
    public TSelf WithCookieOptionsOverride(Action<CookieAuthenticationOptions> apply)
        => (TSelf)(this with { CookieOptionsOverrides = CookieOptionsOverrides.Append(apply).ToList() });

    /// <summary>
    /// Adds a single endpoint assembly to be included in the test harness.
    /// </summary>
    public TSelf WithEndpointAssembly(Assembly assembly)
        => (TSelf)(this with { EndpointAssemblies = EndpointAssemblies.Append(assembly).ToList() });

    /// <summary>
    /// Adds multiple endpoint assemblies to be included in the test harness.
    /// </summary>
    public TSelf WithEndpointAssemblies(params Assembly[] assemblies)
        => (TSelf)(this with { EndpointAssemblies = EndpointAssemblies.Concat(assemblies).ToList() });

    // ------------------------------------------------------------
    // FAKE SERVICE REGISTRATION SUPPORT
    // ------------------------------------------------------------

    /// <summary>
    /// A dictionary of fake service instances keyed by their service type.
    /// Used to override DI registrations with test doubles.
    /// </summary>
    public virtual IReadOnlyDictionary<Type, object> Fakes { get; init; }
        = new Dictionary<Type, object>();

    /// <summary>
    /// Registers a fake instance for a given service type, replacing any existing fake.
    /// </summary>
    /// <typeparam name="TFake">The service type being faked.</typeparam>
    /// <param name="instance">The fake instance to register.</param>
    /// <returns>A mutated copy of the context with the fake registered.</returns>
    public TSelf WithFake<TFake>(TFake instance) where TFake : class
    {
        var copy = new Dictionary<Type, object>(Fakes)
        {
            [typeof(TFake)] = instance
        };

        return (TSelf)(this with { Fakes = copy });
    }

    /// <summary>
    /// Retrieves a previously registered fake instance for the given service type.
    /// Throws an exception if no fake has been registered.
    /// </summary>
    /// <typeparam name="TFake">The service type being retrieved.</typeparam>
    /// <returns>The registered fake instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no fake is registered for the requested type.
    /// </exception>
    public TFake GetFake<TFake>() where TFake : class
    {
        if (!Fakes.TryGetValue(typeof(TFake), out var instance))
            throw new InvalidOperationException($"No fake registered for {typeof(TFake).Name}");

        return (TFake)instance;
    }
}

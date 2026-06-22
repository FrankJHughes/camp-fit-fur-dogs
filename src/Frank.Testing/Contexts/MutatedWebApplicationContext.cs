using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Frank.Testing.Contexts;

public abstract record MutatedWebApplicationContext<TSelf>
    where TSelf : MutatedWebApplicationContext<TSelf>
{
    public string Environment { get; init; } = "Testing";
    public bool DisableDatabase { get; init; }
    public PostgreSqlContainer? Postgres { get; init; }
    public bool UseCookieAuthOnly { get; init; }
    public bool OverrideCookiesForHttp { get; init; } = true;

    public virtual IReadOnlyList<Action<IConfigurationBuilder>> ConfigOverrides { get; init; }
        = Array.Empty<Action<IConfigurationBuilder>>();

    public virtual IReadOnlyList<Action<IServiceCollection>> ServiceOverrides { get; init; }
        = Array.Empty<Action<IServiceCollection>>();

    public virtual IReadOnlyList<Action<CookieAuthenticationOptions>> CookieOptionsOverrides { get; init; }
        = Array.Empty<Action<CookieAuthenticationOptions>>();

    public virtual IReadOnlyList<Assembly> EndpointAssemblies { get; init; }
        = [typeof(Frank.Testing.AssemblyMarker).Assembly];

    // ------------------------------------------------------------
    // FLUENT MUTATION HELPERS
    // ------------------------------------------------------------

    public TSelf WithEnvironment(string environment)
        => (TSelf)(this with { Environment = environment });

    public TSelf WithDatabase(bool enabled, PostgreSqlContainer? postgres)
        => (TSelf)(this with { DisableDatabase = !enabled, Postgres = postgres });

    public TSelf WithCookieAuthOnly(bool value = true)
        => (TSelf)(this with { UseCookieAuthOnly = value });

    public TSelf WithOverrideCookiesForHttp(bool value)
        => (TSelf)(this with { OverrideCookiesForHttp = value });

    public TSelf WithConfigOverride(Action<IConfigurationBuilder> apply)
        => (TSelf)(this with { ConfigOverrides = ConfigOverrides.Append(apply).ToList() });

    public TSelf WithServiceOverride(Action<IServiceCollection> apply)
        => (TSelf)(this with { ServiceOverrides = ServiceOverrides.Append(apply).ToList() });

    public TSelf WithCookieOptionsOverride(Action<CookieAuthenticationOptions> apply)
        => (TSelf)(this with { CookieOptionsOverrides = CookieOptionsOverrides.Append(apply).ToList() });

    public TSelf WithEndpointAssembly(Assembly assembly)
        => (TSelf)(this with { EndpointAssemblies = EndpointAssemblies.Append(assembly).ToList() });

    public TSelf WithEndpointAssemblies(params Assembly[] assemblies)
        => (TSelf)(this with { EndpointAssemblies = EndpointAssemblies.Concat(assemblies).ToList() });

    // ------------------------------------------------------------
    // FAKE SERVICE REGISTRATION SUPPORT
    // ------------------------------------------------------------

    public virtual IReadOnlyDictionary<Type, object> Fakes { get; init; }
        = new Dictionary<Type, object>();

    public TSelf WithFake<TFake>(TFake instance) where TFake : class
    {
        var copy = new Dictionary<Type, object>(Fakes)
        {
            [typeof(TFake)] = instance
        };

        return (TSelf)(this with { Fakes = copy });
    }

    public TFake GetFake<TFake>() where TFake : class
    {
        if (!Fakes.TryGetValue(typeof(TFake), out var instance))
            throw new InvalidOperationException($"No fake registered for {typeof(TFake).Name}");

        return (TFake)instance;
    }
}

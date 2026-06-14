using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiContext(
    string Environment = "Testing",
    bool DisableDatabase = false,
    PostgreSqlContainer? Postgres = null,
    bool UseCookieAuthOnly = false,
    bool OverrideCookiesForHttp = true,
    IReadOnlyList<Action<IConfigurationBuilder>> ConfigOverrides = null!,
    IReadOnlyList<Action<IServiceCollection>> ServiceOverrides = null!,
    IReadOnlyList<Action<CookieAuthenticationOptions>> CookieOptionsOverrides = null!
)
{
    public ApiContext() : this(
        Environment: "Testing",
        DisableDatabase: false,
        Postgres: null,
        UseCookieAuthOnly: false,
        OverrideCookiesForHttp: true,
        ConfigOverrides: Array.Empty<Action<IConfigurationBuilder>>(),
        ServiceOverrides: Array.Empty<Action<IServiceCollection>>(),
        CookieOptionsOverrides: Array.Empty<Action<CookieAuthenticationOptions>>()
    )
    { }

    // -------------------------------------------------------
    // ENVIRONMENT
    // -------------------------------------------------------
    public ApiContext WithEnvironment(string env)
        => this with { Environment = env };

    // -------------------------------------------------------
    // DATABASE
    // -------------------------------------------------------
    public ApiContext WithDatabase(bool enabled, PostgreSqlContainer? container = null)
    {
        if (enabled && container is null)
            throw new ArgumentNullException(nameof(container),
                "Database cannot be enabled without providing a PostgreSqlContainer.");

        return this with
        {
            DisableDatabase = !enabled,
            Postgres = container
        };
    }

    // -------------------------------------------------------
    // COOKIE AUTH ONLY
    // -------------------------------------------------------
    public ApiContext WithCookieAuthOnly(bool enabled)
        => this with { UseCookieAuthOnly = enabled };

    // -------------------------------------------------------
    // COOKIE HTTP OVERRIDE
    // -------------------------------------------------------
    public ApiContext WithCookieHttpOverride(bool enabled)
        => this with { OverrideCookiesForHttp = enabled };

    // -------------------------------------------------------
    // CONFIG OVERRIDES
    // -------------------------------------------------------
    public ApiContext WithConfigOverride(Action<IConfigurationBuilder> apply)
        => this with { ConfigOverrides = ConfigOverrides.Append(apply).ToList() };

    // -------------------------------------------------------
    // SERVICE OVERRIDES
    // -------------------------------------------------------
    public ApiContext WithServiceOverride(Action<IServiceCollection> apply)
        => this with { ServiceOverrides = ServiceOverrides.Append(apply).ToList() };

    // -------------------------------------------------------
    // COOKIE OPTIONS OVERRIDES
    // -------------------------------------------------------
    public ApiContext WithCookieOptionsOverride(Action<CookieAuthenticationOptions> apply)
        => this with { CookieOptionsOverrides = CookieOptionsOverrides.Append(apply).ToList() };
}

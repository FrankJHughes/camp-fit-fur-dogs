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

    public IReadOnlyList<Action<IConfigurationBuilder>> ConfigOverrides { get; init; }
        = Array.Empty<Action<IConfigurationBuilder>>();

    public IReadOnlyList<Action<IServiceCollection>> ServiceOverrides { get; init; }
        = Array.Empty<Action<IServiceCollection>>();

    public IReadOnlyList<Action<CookieAuthenticationOptions>> CookieOptionsOverrides { get; init; }
        = Array.Empty<Action<CookieAuthenticationOptions>>();
}

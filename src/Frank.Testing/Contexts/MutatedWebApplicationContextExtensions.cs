using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Frank.Testing.Contexts;

public static class MutatedWebApplicationContextExtensions
{
    public static TSelf WithEnvironment<TSelf>(this TSelf ctx, string env)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { Environment = env };

    public static TSelf WithDatabase<TSelf>(
        this TSelf ctx,
        bool enabled,
        PostgreSqlContainer? container = null)
        where TSelf : MutatedWebApplicationContext<TSelf>
    {
        if (enabled && container is null)
            throw new ArgumentNullException(nameof(container));

        return ctx with
        {
            DisableDatabase = !enabled,
            Postgres = container
        };
    }

    public static TSelf WithCookieAuthOnly<TSelf>(this TSelf ctx, bool enabled)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { UseCookieAuthOnly = enabled };

    public static TSelf WithCookieHttpOverride<TSelf>(this TSelf ctx, bool enabled)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { OverrideCookiesForHttp = enabled };

    public static TSelf WithConfigOverride<TSelf>(
        this TSelf ctx,
        Action<IConfigurationBuilder> apply)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { ConfigOverrides = ctx.ConfigOverrides.Append(apply).ToList() };

    public static TSelf WithServiceOverride<TSelf>(
        this TSelf ctx,
        Action<IServiceCollection> apply)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { ServiceOverrides = ctx.ServiceOverrides.Append(apply).ToList() };

    public static TSelf WithCookieOptionsOverride<TSelf>(
        this TSelf ctx,
        Action<CookieAuthenticationOptions> apply)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { CookieOptionsOverrides = ctx.CookieOptionsOverrides.Append(apply).ToList() };
}

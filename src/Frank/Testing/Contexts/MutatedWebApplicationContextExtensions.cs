using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Frank.Testing.Contexts;

/// <summary>
/// Provides fluent extension methods for mutating instances of
/// <see cref="MutatedWebApplicationContext{TSelf}"/>.
/// <para>
/// These helpers allow test scenarios to customize environment settings,
/// database behavior, authentication modes, configuration overrides,
/// service overrides, and cookie authentication options.
/// </para>
/// <para>
/// Each method returns a mutated copy of the context, preserving the immutable,
/// record‑based design of the testing infrastructure.
/// </para>
/// </summary>
public static class MutatedWebApplicationContextExtensions
{
    /// <summary>
    /// Sets the hosting environment name for the test application.
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="env">The environment name to apply.</param>
    /// <returns>A mutated copy of the context.</returns>
    public static TSelf WithEnvironment<TSelf>(this TSelf ctx, string env)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { Environment = env };

    /// <summary>
    /// Enables or disables database usage and optionally assigns a PostgreSQL container.
    /// <para>
    /// When enabling database support, a non‑null <paramref name="container"/> is required.
    /// </para>
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="enabled">Whether database usage should be enabled.</param>
    /// <param name="container">The PostgreSQL container to use when enabled.</param>
    /// <returns>A mutated copy of the context.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="enabled"/> is <c>true</c> but <paramref name="container"/> is <c>null</c>.
    /// </exception>
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

    /// <summary>
    /// Configures the test harness to use cookie authentication exclusively.
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="enabled">Whether cookie‑only authentication should be used.</param>
    /// <returns>A mutated copy of the context.</returns>
    public static TSelf WithCookieAuthOnly<TSelf>(this TSelf ctx, bool enabled)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { UseCookieAuthOnly = enabled };

    /// <summary>
    /// Enables or disables cookie overrides for HTTP scenarios.
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="enabled">Whether cookie overrides should be applied.</param>
    /// <returns>A mutated copy of the context.</returns>
    public static TSelf WithCookieHttpOverride<TSelf>(this TSelf ctx, bool enabled)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { OverrideCookiesForHttp = enabled };

    /// <summary>
    /// Adds a configuration override to be applied during test application setup.
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="apply">The configuration mutation to apply.</param>
    /// <returns>A mutated copy of the context.</returns>
    public static TSelf WithConfigOverride<TSelf>(
        this TSelf ctx,
        Action<IConfigurationBuilder> apply)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { ConfigOverrides = ctx.ConfigOverrides.Append(apply).ToList() };

    /// <summary>
    /// Adds a service override to be applied during test application DI setup.
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="apply">The service mutation to apply.</param>
    /// <returns>A mutated copy of the context.</returns>
    public static TSelf WithServiceOverride<TSelf>(
        this TSelf ctx,
        Action<IServiceCollection> apply)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { ServiceOverrides = ctx.ServiceOverrides.Append(apply).ToList() };

    /// <summary>
    /// Adds a cookie authentication options override for customizing cookie behavior.
    /// </summary>
    /// <typeparam name="TSelf">The concrete context type.</typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="apply">The cookie options mutation to apply.</param>
    /// <returns>A mutated copy of the context.</returns>
    public static TSelf WithCookieOptionsOverride<TSelf>(
        this TSelf ctx,
        Action<CookieAuthenticationOptions> apply)
        where TSelf : MutatedWebApplicationContext<TSelf>
        => ctx with { CookieOptionsOverrides = ctx.CookieOptionsOverrides.Append(apply).ToList() };
}

namespace Frank.Testing.Contexts;

/// <summary>
/// Provides fluent extension methods for mutating instances of
/// <see cref="MutatedWebApplicationClientContext{TSelf}"/>.
/// <para>
/// These helpers enable test scenarios to override user identity, authentication
/// schemes, and default request headers in a clean, immutable, record‑based
/// manner. Each method returns a mutated copy of the context, preserving the
/// functional style of the testing infrastructure.
/// </para>
/// </summary>
public static class MutatedWebApplicationClientContextExtensions
{
    /// <summary>
    /// Sets the simulated current user's subject identifier (<c>sub</c>) on the
    /// testing context.
    /// <para>
    /// Useful for emulating authenticated requests or switching between users
    /// within a single test suite.
    /// </para>
    /// </summary>
    /// <typeparam name="TSelf">
    /// The concrete context type inheriting from
    /// <see cref="MutatedWebApplicationClientContext{TSelf}"/>.
    /// </typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="sub">The subject identifier to assign.</param>
    /// <returns>
    /// A mutated copy of the context with <c>CurrentUserSub</c> set.
    /// </returns>
    public static TSelf WithCurrentUser<TSelf>(
        this TSelf ctx,
        string? sub)
        where TSelf : MutatedWebApplicationClientContext<TSelf>
        => ctx with { CurrentUserSub = sub };

    /// <summary>
    /// Adds or replaces a default HTTP header on the testing context.
    /// <para>
    /// This is useful for simulating API keys, correlation IDs, custom metadata,
    /// or any header required across multiple test requests.
    /// </para>
    /// </summary>
    /// <typeparam name="TSelf">
    /// The concrete context type inheriting from
    /// <see cref="MutatedWebApplicationClientContext{TSelf}"/>.
    /// </typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="key">The header name.</param>
    /// <param name="value">The header value.</param>
    /// <returns>
    /// A mutated copy of the context with the updated header set.
    /// </returns>
    public static TSelf WithHeader<TSelf>(
        this TSelf ctx,
        string key,
        string value)
        where TSelf : MutatedWebApplicationClientContext<TSelf>
    {
        var copy = new Dictionary<string, string>(ctx.DefaultHeaders)
        {
            [key] = value
        };
        return ctx with { DefaultHeaders = copy };
    }

    /// <summary>
    /// Sets the authentication scheme used when simulating sign‑in behavior
    /// within the test harness.
    /// <para>
    /// This allows tests to override the default scheme, enabling scenarios such
    /// as cookie authentication, bearer tokens, or custom test schemes.
    /// </para>
    /// </summary>
    /// <typeparam name="TSelf">
    /// The concrete context type inheriting from
    /// <see cref="MutatedWebApplicationClientContext{TSelf}"/>.
    /// </typeparam>
    /// <param name="ctx">The context to mutate.</param>
    /// <param name="scheme">The sign‑in scheme to apply.</param>
    /// <returns>
    /// A mutated copy of the context with <c>SignInScheme</c> set.
    /// </returns>
    public static TSelf WithSignInScheme<TSelf>(
        this TSelf ctx,
        string scheme)
        where TSelf : MutatedWebApplicationClientContext<TSelf>
        => ctx with { SignInScheme = scheme };
}

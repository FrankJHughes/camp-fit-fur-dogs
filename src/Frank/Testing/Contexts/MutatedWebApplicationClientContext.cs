namespace Frank.Testing.Contexts;

/// <summary>
/// Represents a mutated testing context for a web application client,
/// allowing test scenarios to override or inject user‑related state and
/// default request headers.
/// <para>
/// This abstraction is used in integration and functional tests where the
/// test harness needs to simulate authenticated users, custom sign‑in schemes,
/// or preconfigured HTTP headers.
/// </para>
/// <para>
/// The generic <typeparamref name="TSelf"/> pattern enables fluent,
/// self‑referencing mutation APIs in derived test contexts.
/// </para>
/// </summary>
/// <typeparam name="TSelf">
/// The concrete context type inheriting from this record, enabling
/// self‑referencing fluent mutation patterns.
/// </typeparam>
public abstract record MutatedWebApplicationClientContext<TSelf>
    where TSelf : MutatedWebApplicationClientContext<TSelf>
{
    /// <summary>
    /// The subject identifier (<c>sub</c>) of the simulated current user.
    /// <para>
    /// When set, test scenarios can emulate authenticated requests by injecting
    /// this value into the test harness’s authentication pipeline.
    /// </para>
    /// </summary>
    public string? CurrentUserSub { get; init; }

    /// <summary>
    /// The sign‑in scheme to use when simulating authentication.
    /// <para>
    /// This allows tests to override the default authentication scheme used by
    /// the application, enabling scenarios such as cookie auth, bearer tokens,
    /// or custom test schemes.
    /// </para>
    /// </summary>
    public string? SignInScheme { get; init; }

    /// <summary>
    /// A collection of default HTTP headers applied to every request issued by
    /// the mutated web application client.
    /// <para>
    /// Useful for simulating API keys, correlation IDs, custom test metadata,
    /// or any headers required across multiple test cases.
    /// </para>
    /// </summary>
    public Dictionary<string, string> DefaultHeaders { get; init; } = new();
}

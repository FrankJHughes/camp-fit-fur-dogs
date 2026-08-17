using Frank.Core.Application.Abstractions.EnvironmentVariables;

namespace Frank.Core.Infrastructure.EnvironmentVariables;

/// <summary>
/// Provides the infrastructure‑level implementation of
/// <see cref="IEnvironmentVariables"/>, retrieving values from the host
/// operating system's environment variable store.
/// <para>
/// This implementation is suitable for production scenarios where environment
/// variables are used for configuration, secrets, or runtime toggles.
/// Test environments may substitute this with an in‑memory or mock
/// implementation to ensure deterministic behavior.
/// </para>
/// </summary>
public sealed class SystemEnvironmentVariables : IEnvironmentVariables
{
    /// <summary>
    /// Retrieves the value of the specified environment variable from the
    /// underlying operating system.
    /// </summary>
    /// <param name="key">
    /// The name of the environment variable to retrieve.
    /// </param>
    /// <returns>
    /// The value of the environment variable, or <c>null</c> if the variable
    /// does not exist.
    /// </returns>
    public string? Get(string key) => System.Environment.GetEnvironmentVariable(key);
}

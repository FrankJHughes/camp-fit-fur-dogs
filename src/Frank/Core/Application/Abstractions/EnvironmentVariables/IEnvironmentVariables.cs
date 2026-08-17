namespace Frank.Core.Application.Abstractions.EnvironmentVariables;

/// <summary>
/// Provides access to environment variables used by the application.
///
/// <para>
/// This abstraction centralizes environment variable retrieval, allowing
/// implementations to apply validation, normalization, defaulting, or
/// security‑related behavior as needed. It also improves testability by
/// decoupling environment access from system APIs.
/// </para>
/// </summary>
public interface IEnvironmentVariables
{
    /// <summary>
    /// Retrieves the value of the specified environment variable.
    ///
    /// <para>
    /// Returns <c>null</c> if the variable is not defined or cannot be read.
    /// Implementations may apply additional logic such as prefixing, caching,
    /// or fallback resolution.
    /// </para>
    /// </summary>
    /// <param name="key">
    /// The name of the environment variable to retrieve.
    /// </param>
    /// <returns>
    /// The value of the environment variable, or <c>null</c> if it is not set.
    /// </returns>
    string? Get(string key);
}

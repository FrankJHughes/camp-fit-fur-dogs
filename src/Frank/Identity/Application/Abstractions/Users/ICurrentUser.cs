namespace Frank.Identity.Application.Abstractions.Users;

/// <summary>
/// Represents the current authenticated user within the application context.
/// <para>
/// This abstraction provides a minimal, application‑layer view of the user
/// associated with the active request.
/// It is typically populated by the authentication middleware or session
/// evaluation pipeline.
/// </para>
/// <para>
/// The interface intentionally exposes only three properties:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="IsAuthenticated"/> — whether a user is authenticated</description></item>
/// <item><description><see cref="Id"/> — the internal user identifier, if available</description></item>
/// <item><description><see cref="Name"/> — the user's display name, if available</description></item>
/// </list>
/// <para>
/// This keeps the current‑user context lightweight, safe, and free of sensitive
/// information. Additional user details should be retrieved through dedicated
/// query abstractions such as <c>IGetUserByIdReader</c>.
/// </para>
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Indicates whether the current request is associated with an authenticated
    /// user.
    /// When <c>false</c>, both <see cref="Id"/> and <see cref="Name"/> should be
    /// considered unavailable.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// The internal unique identifier of the authenticated user, or <c>null</c>
    /// if the request is unauthenticated.
    /// </summary>
    Guid? Id { get; }

    /// <summary>
    /// The display name of the authenticated user, or <c>null</c> if unavailable
    /// or unauthenticated.
    /// </summary>
    string? Name { get; }
}

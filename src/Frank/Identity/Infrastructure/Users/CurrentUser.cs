using System.Security.Claims;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Http;

namespace Frank.Identity.Infrastructure.Users;

/// <summary>
/// Provides access to information about the currently authenticated user
/// based on the <see cref="HttpContext.User"/> principal.
/// <para>
/// This implementation is a thin wrapper around <see cref="IHttpContextAccessor"/>
/// and exposes strongly typed access to common identity values such as:
/// </para>
/// <list type="bullet">
/// <item><description>Authentication status</description></item>
/// <item><description>User identifier (<c>sub</c> / <c>NameIdentifier</c>)</description></item>
/// <item><description>User display name</description></item>
/// </list>
/// <para>
/// The class is designed for use within the Identity subsystem and application
/// services that require information about the current request’s authenticated user.
/// </para>
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class using
    /// the provided <see cref="IHttpContextAccessor"/>.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// Provides access to the current <see cref="HttpContext"/> and its associated
    /// <see cref="ClaimsPrincipal"/>.
    /// </param>
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Indicates whether the current request is associated with an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true;
        }
    }

    /// <summary>
    /// Gets the authenticated user's unique identifier, typically sourced from the
    /// <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// <para>
    /// Returns <c>null</c> if the claim is missing or cannot be parsed as a <see cref="Guid"/>.
    /// </para>
    /// </summary>
    public Guid? Id
    {
        get
        {
            var claimValue = GetClaimValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claimValue, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Gets the authenticated user's display name, typically sourced from the
    /// <see cref="ClaimTypes.Name"/> claim.
    /// <para>
    /// Returns <c>null</c> if the claim is missing.
    /// </para>
    /// </summary>
    public string? Name
    {
        get
        {
            var claimValue = GetClaimValue(ClaimTypes.Name);
            return claimValue;
        }
    }

    /// <summary>
    /// Retrieves the value of a specific claim from the current user's
    /// <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="key">The claim type to retrieve.</param>
    /// <returns>
    /// The claim value if present; otherwise <c>null</c>.
    /// </returns>
    private string? GetClaimValue(string key)
    {
        var user = GetClaimsPrincipal();
        return user?.FindFirst(key)?.Value;
    }

    /// <summary>
    /// Retrieves the <see cref="ClaimsPrincipal"/> associated with the current request.
    /// </summary>
    /// <returns>
    /// The current <see cref="ClaimsPrincipal"/>, or <c>null</c> if no HTTP context exists.
    /// </returns>
    private ClaimsPrincipal? GetClaimsPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}

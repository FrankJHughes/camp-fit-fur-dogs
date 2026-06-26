using System.Security.Claims;
using Frank.Abstractions.Identity;
using Microsoft.AspNetCore.Http;

namespace Frank.Infrastructure.Identity;

public sealed class AuthenticatedUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return null;
            }

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(id, out _);
        }
    }
}

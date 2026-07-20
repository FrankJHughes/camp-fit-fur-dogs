using System.Security.Claims;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Http;

namespace Frank.Identity.Infrastructure.Users;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true;
        }
    }

    public Guid? Id
    {
        get
        {
            var claimValue = GetClaimValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claimValue, out var id) ? id : null;
        }
    }

    public string? Name
    {
        get
        {
            var claimValue = GetClaimValue(ClaimTypes.Name);
            return claimValue;
        }
    }

    private string? GetClaimValue(string key)
    {
        var user = GetClaimsPrincipal();
        return user?.FindFirst(key)?.Value;
    }

    private ClaimsPrincipal? GetClaimsPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

}

using System.Security.Claims;
using CampFitFurDogs.Application.Exceptions;
using Frank.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Infrastructure.Identity;

public sealed class AuthenticatedUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid Id
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UserNotAuthenticatedException();

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst("sub")?.Value;

            if (id is null)
                throw new UserIdClaimNotFoundException();

            return Guid.Parse(id);
        }
    }
}

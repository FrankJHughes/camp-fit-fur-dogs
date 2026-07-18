using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Users;

public sealed class GetUserByExternalIdReader : IGetUserByExternalIdReader
{
    private readonly FrankIdentityDbContext _db;

    public GetUserByExternalIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public Task<GetUserByExternalIdResponse?> GetByExternalIdAsync(
        string externalId,
        CancellationToken ct)
    {
        return _db.Set<User>()
            .AsNoTracking()
            .Where(c =>
                c.ExternalId != null &&
                c.ExternalId.Value == externalId)
            .Select(c =>
                new GetUserByExternalIdResponse(
                    Id: c.Id.Value))
            .SingleOrDefaultAsync(ct);
    }
}

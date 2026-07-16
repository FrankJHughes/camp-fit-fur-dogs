using Frank.Identity.Application.Abstractions.Users.FindUserByExternalId;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Users;

public sealed class FindUserByExternalIdReader : IFindUserByExternalIdReader
{
    private readonly FrankIdentityDbContext _db;

    public FindUserByExternalIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public Task<FindUserByExternalIdResponse?> FindByExternalIdAsync(
        string externalId,
        CancellationToken ct)
    {
        return _db.Set<User>()
            .AsNoTracking()
            .Where(c =>
                c.ExternalId != null &&
                c.ExternalId.Value == externalId)
            .Select(c =>
                new FindUserByExternalIdResponse(
                    Id: c.Id.Value))
            .SingleOrDefaultAsync(ct);
    }
}

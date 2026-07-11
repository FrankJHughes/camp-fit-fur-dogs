using Frank.Application.Abstractions.Users.FindUserByExternalId;
using Frank.Domain.Users;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frank.Infrastructure.EntityFrameworkCore.Users;

public sealed class FindUserByExternalIdReader : IFindUserByExternalIdReader
{
    private readonly FrankIdentityDbContext _db;

    public FindUserByExternalIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public Task<FindUsererByExternalIdResponse?> FindByExternalIdAsync(
        string externalId,
        CancellationToken ct)
    {
        return _db.Set<User>()
            .AsNoTracking()
            .Where(c =>
                c.ExternalId != null &&
                c.ExternalId.Value == externalId)
            .Select(c =>
                new FindUsererByExternalIdResponse(
                    Id: c.Id.Value))
            .SingleOrDefaultAsync(ct);
    }
}

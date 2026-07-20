using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Users;

public sealed class GetUserByIdReader : IGetUserByIdReader
{
    private readonly FrankIdentityDbContext _db;

    public GetUserByIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public Task<GetUserByIdResponse?> ReadAsync(
        Guid userId,
        CancellationToken ct)
    {
        return _db.Set<User>()
            .AsNoTracking()
            .Where(c =>
                c.Id == UserId.From(userId))
            .Select(c =>
                new GetUserByIdResponse(
                    Id: c.Id.Value,
                    FirstName: c.FirstName.Value,
                    LastName: c.LastName.Value))
            .SingleOrDefaultAsync(ct);
    }
}

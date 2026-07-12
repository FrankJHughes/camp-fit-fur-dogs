using Frank.Application.Abstractions.Users.GetUserById;
using Frank.Domain.Users;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frank.Infrastructure.EntityFrameworkCore.Users;

public sealed class GetUserByIdReader : IGetUserByIdReader
{
    private readonly FrankIdentityDbContext _db;

    public GetUserByIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public Task<GetUserByIdResponse?> GetByIdAsync(
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

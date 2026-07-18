using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.Users;

public sealed class CreateUserWriter : ICreateUserWriter
{
    private readonly FrankIdentityDbContext _db;

    public CreateUserWriter(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(User user, CancellationToken ct)
    {
        await _db.Set<User>().AddAsync(user, ct);
    }
}

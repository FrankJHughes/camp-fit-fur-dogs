using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.Persistence;

namespace Frank.Identity.EntityFrameworkCore.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly FrankIdentityDbContext _db;

    public UserRepository(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _db.Set<User>().AddAsync(user, ct);
    }
}

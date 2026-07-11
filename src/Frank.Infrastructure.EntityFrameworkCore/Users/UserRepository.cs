using Frank.Domain.Users;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;

namespace Frank.Infrastructure.EntityFrameworkCore.Users;

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

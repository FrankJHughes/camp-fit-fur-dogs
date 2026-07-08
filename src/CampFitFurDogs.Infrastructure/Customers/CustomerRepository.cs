using Frank.Domain.Users;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure.Customers;

public sealed class CustomerRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User customer, CancellationToken ct)
    {
        await _db.Set<User>().AddAsync(customer, ct);
    }
}

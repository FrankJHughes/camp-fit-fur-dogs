using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure.Customers;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Domain.Customers.Customer customer, CancellationToken ct)
    {
        await _db.Set<Domain.Customers.Customer>().AddAsync(customer, ct);
    }
}

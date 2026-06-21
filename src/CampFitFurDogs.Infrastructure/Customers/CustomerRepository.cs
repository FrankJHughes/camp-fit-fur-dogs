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

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _db.Set<Customer>().AddAsync(customer, ct);
    }
}

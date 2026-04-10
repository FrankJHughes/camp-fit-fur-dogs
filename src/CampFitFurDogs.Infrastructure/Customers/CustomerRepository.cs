using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken ct)
    {
        return await _db.Customers
            .AnyAsync(c => c.Email.Value == email.Value, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _db.Customers.AddAsync(customer, ct);
    }
}

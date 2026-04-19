using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        return await _db.Set<Customer>().AnyAsync(c => c.Email.Value == email.Value, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _db.Set<Customer>().AddAsync(customer, ct);
    }
}

using CampFitFurDogs.Application.Abstractions.Customer.GetCustomerById;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Customers;

public sealed class GetCustomerByIdReader : IGetCustomerByIdReader
{
    private readonly AppDbContext _db;

    public GetCustomerByIdReader(AppDbContext db)
    {
        _db = db;
    }

    public Task<GetCustomerByIdResponse?> GetByIdAsync(
        Guid customerId,
        CancellationToken ct)
    {
        return _db.Set<Customer>()
            .AsNoTracking()
            .Where(c =>
                c.Id == CustomerId.From(customerId))
            .Select(c =>
                new GetCustomerByIdResponse(
                    Id: c.Id.Value,
                    FirstName: c.FirstName.Value,
                    LastName: c.LastName.Value))
            .SingleOrDefaultAsync(ct);
    }
}

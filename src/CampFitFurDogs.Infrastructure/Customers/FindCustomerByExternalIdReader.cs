using CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Customers;

public sealed class FindCustomerByExternalIdReader : IFindCustomerByExternalIdReader
{
    private readonly AppDbContext _db;

    public FindCustomerByExternalIdReader(AppDbContext db)
    {
        _db = db;
    }

    public Task<FindCustomerByExternalIdResponse?> FindByExternalIdAsync(
        string externalId,
        CancellationToken ct)
    {
        return _db.Set<Customer>()
            .AsNoTracking()
            .Where(c =>
                c.ExternalId != null &&
                c.ExternalId.Value == externalId)
            .Select(c =>
                new FindCustomerByExternalIdResponse(
                    Id: c.Id.Value))
            .SingleOrDefaultAsync(ct);
    }
}

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
        // IMPORTANT:
        // EF Core cannot translate VO construction inside the query.
        // Also, comparing VO instances with == does NOT work unless operator overloads exist.
        // Therefore we compare on the underlying string value.
        var externalAuthId = ExternalAuthProviderId.From(externalId);

        return _db.Set<Customer>()
            .AsNoTracking()
            .Where(c =>
                c.ExternalAuthProviderId != null &&
                c.ExternalAuthProviderId == externalAuthId)
            .Select(c =>
                new FindCustomerByExternalIdResponse(
                    Id: c.Id.Value))
            .SingleOrDefaultAsync(ct);
    }
}

using CampFitFurDogs.Application.Abstractions.Customer.FindCustomerByExternalId;

namespace CampFitFurDogs.Infrastructure.Tests.Fakes;

public sealed class FakeFindCustomerByExternalIdReader : IFindCustomerByExternalIdReader
{
    public FindCustomerByExternalIdResponse? Response { get; set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task<FindCustomerByExternalIdResponse?> FindByExternalIdAsync(string externalAuthProviderId, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(Response);
    }
}

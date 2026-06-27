namespace CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;

public interface IFindCustomerByExternalIdReader
{
    Task<FindCustomerByExternalIdResponse?> FindByExternalIdAsync(
        string externalId, CancellationToken cancellationToken);
}

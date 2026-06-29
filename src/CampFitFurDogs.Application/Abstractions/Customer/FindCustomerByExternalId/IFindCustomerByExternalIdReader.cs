namespace CampFitFurDogs.Application.Abstractions.Customer.FindCustomerByExternalId;

public interface IFindCustomerByExternalIdReader
{
    Task<FindCustomerByExternalIdResponse?> FindByExternalIdAsync(
        string externalId, CancellationToken cancellationToken);
}

using Microsoft.Extensions.DependencyInjection;

using Frank.DependencyInjection;
namespace CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IFindCustomerByExternalIdReader
{
    Task<FindCustomerByExternalIdResponse?> FindByExternalIdAsync(
        string externalId, CancellationToken cancellationToken);
}

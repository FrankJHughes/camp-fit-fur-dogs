using Microsoft.Extensions.DependencyInjection;

using SharedKernel.DependencyInjection;

namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IListDogsByOwnerReader
{
    Task<ListDogsByOwnerResponse> ListDogsByOwnerAsync(
        Guid ownerId, CancellationToken ct);
}

using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IListDogsByOwnerReader
{
    Task<ListDogsByOwnerResponse> ListDogsByOwnerAsync(
        Guid ownerId, CancellationToken ct);
}

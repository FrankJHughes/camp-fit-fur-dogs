
using Microsoft.Extensions.DependencyInjection;

using Frank.DependencyInjection;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]

public interface IGetDogProfileReader
{
    Task<GetDogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct);
}

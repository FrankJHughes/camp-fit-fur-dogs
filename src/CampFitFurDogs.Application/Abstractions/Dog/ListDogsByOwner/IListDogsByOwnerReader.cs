namespace CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;

public interface IListDogsByOwnerReader
{
    Task<ListDogsByOwnerResponse> ListDogsByOwnerAsync(
        Guid ownerId, CancellationToken ct);
}

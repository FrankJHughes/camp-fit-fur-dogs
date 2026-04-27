namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

public interface IListDogsByOwnerReader
{
    Task<ListDogsByOwnerResponse> ListDogsByOwnerAsync(
        Guid ownerId, CancellationToken ct);
}

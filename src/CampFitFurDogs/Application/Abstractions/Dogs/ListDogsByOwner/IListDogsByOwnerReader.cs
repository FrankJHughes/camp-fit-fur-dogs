namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

public interface IListDogsByOwnerReader
{
    Task<ListDogsByOwnerResponse> ReadAsync(
        Guid ownerId, CancellationToken ct);
}

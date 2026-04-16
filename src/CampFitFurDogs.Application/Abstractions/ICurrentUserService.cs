namespace CampFitFurDogs.Application.Abstractions;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}

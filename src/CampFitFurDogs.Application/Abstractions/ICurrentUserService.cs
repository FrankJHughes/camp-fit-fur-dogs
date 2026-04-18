namespace CampFitFurDogs.Application.Abstractions;

public interface ICurrentUserService
{
    Guid CurrentUserId { get; }
}

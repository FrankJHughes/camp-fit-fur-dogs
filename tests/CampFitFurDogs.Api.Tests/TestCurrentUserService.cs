using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Api.Tests;

public class TestCurrentUserService : ICurrentUserService
{
    public Guid CurrentUserId { get; set; } = Guid.NewGuid();
    public Guid GetCurrentUserId() => CurrentUserId;
}

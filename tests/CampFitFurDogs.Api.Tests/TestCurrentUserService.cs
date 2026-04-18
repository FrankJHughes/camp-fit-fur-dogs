using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Api.Tests;

public sealed class TestCurrentUserService : ICurrentUserService
{
    public Guid CurrentUserId { get; set; } = Guid.NewGuid();
}

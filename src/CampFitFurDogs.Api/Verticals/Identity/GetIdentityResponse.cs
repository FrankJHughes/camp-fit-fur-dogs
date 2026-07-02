namespace CampFitFurDogs.Api.Verticals.Identity;

public sealed class GetIdentityResponse
{
    public bool IsAuthenticated { get; init; }

    public IdentityUserDto? User { get; init; }
}

public sealed class IdentityUserDto
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
}

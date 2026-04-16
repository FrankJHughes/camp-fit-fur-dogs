using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Infrastructure;

public sealed class DummyCurrentUserService : ICurrentUserService
{
    // Hardcoded placeholder for Sprint 4 (pre-auth).
    // Swap for a real implementation when authentication lands.
    private static readonly Guid PlaceholderId =
        Guid.Parse("00000000-0000-0000-0000-000000000001");

    public Guid GetCurrentUserId() => PlaceholderId;
}

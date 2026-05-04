using SharedKernel.Abstractions;

namespace CampFitFurDogs.Infrastructure;

public sealed class DummyCurrentUserService : ICurrentUserService
{
    // Hardcoded placeholder for Sprint 4 (pre-auth).
    // Swap for a real implementation when authentication lands.
    private static readonly Guid PlaceholderId =
        Guid.Parse("d37a2a6b-c581-490d-89ce-f60d73800732");

    public Guid CurrentUserId => PlaceholderId;
}

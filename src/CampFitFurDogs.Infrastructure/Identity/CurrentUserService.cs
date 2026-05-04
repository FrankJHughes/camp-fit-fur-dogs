using SharedKernel.Abstractions;

namespace CampFitFurDogs.Infrastructure;

public sealed class DummyCurrentUserService : ICurrentUserService
{
    // Hardcoded placeholder for Sprint 4 (pre-auth).
    // Swap for a real implementation when authentication lands.
    private static readonly Guid PlaceholderId =
        Guid.Parse("8e390654-a834-4594-8f83-0c6838b0a50c");

    public Guid CurrentUserId => PlaceholderId;
}

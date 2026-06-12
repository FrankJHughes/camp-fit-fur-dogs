using CampFitFurDogs.Api.Horizontals.Hosting.Modules;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeRenderPrParser : IRenderPrParser
{
    public bool TryParse(string renderServiceName, out string? prNumber)
    {
        // Matches the real parser logic: last segment after hyphens
        prNumber = null;

        if (string.IsNullOrWhiteSpace(renderServiceName))
            return false;

        var parts = renderServiceName.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            return false;

        prNumber = parts.Last();
        return true;
    }
}

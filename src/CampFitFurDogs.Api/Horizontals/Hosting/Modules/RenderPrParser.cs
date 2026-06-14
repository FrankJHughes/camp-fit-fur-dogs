namespace CampFitFurDogs.Api.Horizontals.Hosting.Modules;

public sealed class RenderPrParser : IRenderPrParser
{
    public bool TryParse(string? renderServiceName, out string? prNumber)
    {
        prNumber = null;

        if (string.IsNullOrWhiteSpace(renderServiceName))
            return false;

        var parts = renderServiceName.Split(
            ['-'], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return false;

        prNumber = parts.Last();
        return true;
    }
}

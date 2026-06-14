namespace CampFitFurDogs.Api.Horizontals.Hosting.Modules;

public interface IRenderPrParser
{
    bool TryParse(string renderServiceName, out string? prNumber);
}

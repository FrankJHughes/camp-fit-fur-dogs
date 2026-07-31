namespace CampFitFurDogs.Api.PlatformModules;

public interface IRenderPrParser
{
    bool TryParse(string renderServiceName, out string? prNumber);
}

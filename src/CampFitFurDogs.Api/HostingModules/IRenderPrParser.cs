namespace CampFitFurDogs.Api.HostingModules;

public interface IRenderPrParser
{
    bool TryParse(string renderServiceName, out string? prNumber);
}

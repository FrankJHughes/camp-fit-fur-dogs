namespace CampFitFurDogs.Api.Horizontal.Hosting.Modules;

public interface IRenderPrParser
{
    bool TryParse(string renderServiceName, out string? prNumber);
}

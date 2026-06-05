namespace CampFitFurDogs.Api.Hosting;

public interface IRenderPrParser
{
    bool TryParse(string renderServiceName, out string? prNumber);
}

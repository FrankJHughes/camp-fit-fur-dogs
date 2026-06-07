namespace CampFitFurDogs.Api.Horizontals.Hosting;

public interface IRenderPrParser
{
    bool TryParse(string renderServiceName, out string? prNumber);
}

namespace CampFitFurDogs.Api.IntegrationTests;

public abstract class ApiTestBase
{
    protected HttpClient Client { get; }

    protected ApiTestBase()
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
            ?? throw new InvalidOperationException("API_BASE_URL not set");
        Console.WriteLine($"Using API Base URL: {baseUrl}");

        Client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }
}

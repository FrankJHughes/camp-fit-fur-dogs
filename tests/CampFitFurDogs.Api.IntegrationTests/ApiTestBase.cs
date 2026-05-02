using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CampFitFurDogs.Api.IntegrationTests;

public abstract class ApiTestBase : IAsyncLifetime
{
    protected HttpClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("API_BASE_URL environment variable is not set.");

        Client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }
}

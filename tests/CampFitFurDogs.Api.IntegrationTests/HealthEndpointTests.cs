using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CampFitFurDogs.Api.IntegrationTests;

public class HealthEndpointTests : ApiTestBase
{
    [Fact]
    public async Task Health_endpoint_returns_200()
    {
        var response = await Client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

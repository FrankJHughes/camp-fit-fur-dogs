using System.Net;
using Frank.Api.Tests.Helpers;
using Xunit;

namespace Frank.Api.Tests.Observability;

public class CorrelationTests : ObservabilityTestBase
{
    [Fact]
    public async Task Returns_CorrelationId_Header()
    {
        var response = await Client.GetAsync("/__test__/correlation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.True(
            response.Headers.Contains("X-Correlation-Id"),
            "Correlation ID header should be present"
        );
    }
}

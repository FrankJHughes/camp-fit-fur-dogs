using System.Net;
using Frank.Api.Tests.Fakes;
using Frank.Api.Tests.Helpers;
using Xunit;

namespace Frank.Api.Tests.Observability;

public class MetricsTests : ObservabilityTestBase
{
    [Fact]
    public async Task Increments_Metrics_On_Request()
    {
        var response = await Client.GetAsync("/__test__/metrics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(Metrics.Count > 0);
    }
}

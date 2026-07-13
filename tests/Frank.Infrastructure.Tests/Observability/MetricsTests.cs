using System.Net;
using Frank.Core.Infrastructure.Tests.Observability.Helpers;
using Xunit;

namespace Frank.Core.Api.Tests.Observability;

public class MetricsTests : ObservabilityTestBase
{
    [Fact]
    public async Task Records_Metrics_On_Successful_Request()
    {
        var response = await Client.GetAsync("/__test__/metrics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(Metrics.Count > 0);
    }

    [Fact]
    public async Task Records_Metrics_On_Error_Request()
    {
        var response = await Client.GetAsync("/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(Metrics.Count > 0);
    }
}

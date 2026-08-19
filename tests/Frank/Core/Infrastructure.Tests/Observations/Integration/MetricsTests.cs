using System.Net;
using Frank.TestUtilities.Helpers;

namespace Frank.Core.Infrastructure.Tests.Observations.Integration;

public class MetricsTests : ObservationsTestBase
{
    [Fact]
    public async Task Records_Metrics_On_Successful_Request()
    {
        var response = await Client.GetAsync("/api/__test__/metrics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(Metrics.Count > 0);
    }

    [Fact]
    public async Task Records_Metrics_On_Error_Request()
    {
        var response = await Client.GetAsync("/api/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(Metrics.Count > 0);
    }
}

using System.Net;
using Frank.TestUtilities.Helpers;

namespace Frank.Core.Infrastructure.Tests.Observations.Integration;

public class ExceptionFlowTests : ObservationsTestBase
{
    [Fact]
    public async Task Exception_Flows_Through_Observability_Pipeline()
    {
        var response = await Client.GetAsync("/api/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        Assert.NotEmpty(Trace.Events);
        Assert.NotEmpty(Errors.Errors);
        Assert.True(Metrics.Count > 0);
    }
}

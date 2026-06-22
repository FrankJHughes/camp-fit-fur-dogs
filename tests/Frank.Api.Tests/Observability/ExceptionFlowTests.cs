using System.Net;
using Frank.Api.Tests.Fakes;
using Frank.Api.Tests.Helpers;
using Xunit;

namespace Frank.Api.Tests.Observability;

public class ExceptionFlowTests : ObservabilityTestBase
{
    [Fact]
    public async Task Exception_Flows_Through_Observability_Pipeline()
    {
        var response = await Client.GetAsync("/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        Assert.NotEmpty(Trace.Events);
        Assert.NotEmpty(Errors.Errors);
        Assert.True(Metrics.Count > 0);
    }
}

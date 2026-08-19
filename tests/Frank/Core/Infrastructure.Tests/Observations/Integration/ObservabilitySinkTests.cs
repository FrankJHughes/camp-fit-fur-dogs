using System.Net;
using Frank.TestUtilities.Helpers;

namespace Frank.Core.Infrastructure.Tests.Observations.Integration;

public class ObservabilitySinkTests : ObservationsTestBase
{
    [Fact]
    public async Task Emits_TraceEvents_On_Successful_Request()
    {
        var response = await Client.GetAsync("/api/__test__/trace");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(Trace.Events);
    }

    [Fact]
    public async Task Emits_TraceEvents_On_Error_Request()
    {
        var response = await Client.GetAsync("/api/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotEmpty(Trace.Events);

        Assert.Contains(
            Trace.Events,
            e => e.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)
        );
    }
}

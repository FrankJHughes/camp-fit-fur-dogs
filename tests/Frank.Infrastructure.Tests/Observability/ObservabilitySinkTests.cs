using System.Net;
using Frank.Infrastructure.Tests.Observability.Helpers;
using Xunit;

namespace Frank.Api.Tests.Observability;

public class ObservabilitySinkTests : ObservabilityTestBase
{
    [Fact]
    public async Task Emits_TraceEvents_On_Successful_Request()
    {
        var response = await Client.GetAsync("/__test__/trace");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(Trace.Events);
    }

    [Fact]
    public async Task Emits_TraceEvents_On_Error_Request()
    {
        var response = await Client.GetAsync("/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotEmpty(Trace.Events);

        Assert.Contains(
            Trace.Events,
            e => e.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)
        );
    }
}

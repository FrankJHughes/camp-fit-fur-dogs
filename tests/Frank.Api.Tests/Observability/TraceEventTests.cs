using System.Net;
using Frank.Api.Tests.Helpers;
using Xunit;

namespace Frank.Api.Tests.Observability;

public class TraceEventTests : ObservabilityTestBase
{
    [Fact]
    public async Task Emits_TraceEvents_When_Endpoint_Is_Called()
    {
        var response = await Client.GetAsync("/__test__/trace");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(Trace.Events);
    }
}

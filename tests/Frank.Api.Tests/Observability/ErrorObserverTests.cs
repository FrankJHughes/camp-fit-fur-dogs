using System.Net;
using Frank.Api.Tests.Helpers;
using Xunit;

namespace Frank.Api.Tests.Observability;

public class ErrorObserverTests : ObservabilityTestBase
{
    [Fact]
    public async Task Captures_Errors_When_Exception_Is_Thrown()
    {
        var response = await Client.GetAsync("/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotEmpty(Errors.Errors);
    }
}

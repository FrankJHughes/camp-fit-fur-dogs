using System.Net;
using Frank.TestUtilities.Helpers;

namespace Frank.Core.Infrastructure.Tests.Observations.Integration;

public class ErrorObserverTests : ObservationsTestBase
{
    [Fact]
    public async Task Captures_Errors_When_Exception_Is_Thrown()
    {
        var response = await Client.GetAsync("/api/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotEmpty(Errors.Errors);
    }
}

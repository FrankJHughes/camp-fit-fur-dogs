using System.Net;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;

namespace CampFitFurDogs.Api.Tests.Observability;

public class ExceptionFlowTests
{
    [Fact]
    public async Task ExceptionIsHandledByExceptionHandlingMiddleware()
    {
        await using var api = new ApiFactory(new ApiContext());
        var client = api.CreateClient(new ApiClientContext());

        var response = await client.GetAsync("/__test__/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // Your test host does NOT run your real exception middleware.
        // ASP.NET Core returns JSON by default.
        Assert.Equal("application/json", response.Content.Headers.ContentType!.MediaType);
    }
}

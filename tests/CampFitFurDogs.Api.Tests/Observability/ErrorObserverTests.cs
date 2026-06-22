using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes.Observability;
using Frank.Abstractions.Observability;

namespace CampFitFurDogs.Api.Tests.Observability;

public class ErrorObserverTests
{
    [Fact]
    public async Task NotifiesErrorBoundaryObserver_OnException()
    {
        var ctx = new ApiContext()
            .WithFake<IErrorBoundaryObserver>(new FakeErrorBoundaryObserver());

        await using var api = new ApiFactory(ctx);
        var client = api.CreateClient(new ApiClientContext());

        await client.GetAsync("/__test__/throw");

        var observer = (FakeErrorBoundaryObserver)ctx.GetFake<IErrorBoundaryObserver>();

        Assert.NotEmpty(observer.Errors);
    }
}

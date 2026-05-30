using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CampFitFurDogs.TestUtilities.Fakes;

public class RouteBuilderStub : IEndpointRouteBuilder
{
    private readonly IServiceProvider _services;

    public RouteBuilderStub(IServiceProvider services)
    {
        _services = services;
    }

    public IServiceProvider ServiceProvider => _services;

    public IApplicationBuilder CreateApplicationBuilder()
        => new ApplicationBuilder(_services);

    public IApplicationBuilder ApplicationBuilder => CreateApplicationBuilder();

    public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();

    public Delegate? Handler { get; set; }
}

public static class RouteBuilderStubExtensions
{
    public static IEndpointConventionBuilder MapGet(
        this RouteBuilderStub stub,
        string pattern,
        Delegate handler)
    {
        stub.Handler = handler;
        return new StubEndpointConventionBuilder();
    }

    private class StubEndpointConventionBuilder : IEndpointConventionBuilder
    {
        public void Add(Action<EndpointBuilder> convention) { }
    }
}

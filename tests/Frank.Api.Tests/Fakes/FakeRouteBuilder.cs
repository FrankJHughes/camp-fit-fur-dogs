using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Frank.Api.Tests.Fakes;

public sealed class FakeRouteBuilder : IEndpointRouteBuilder
{
    public FakeRouteBuilder(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        DataSources = [];
    }

    public IServiceProvider ServiceProvider { get; }

    public ICollection<EndpointDataSource> DataSources { get; }

    public IApplicationBuilder CreateApplicationBuilder()
        => new ApplicationBuilder(ServiceProvider);
}

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Api.Tests.Fakes;

public sealed class FakeRouteBuilder : IEndpointRouteBuilder
{
    public IServiceProvider ServiceProvider { get; } = new ServiceCollection().BuildServiceProvider();
    public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();

    public IApplicationBuilder CreateApplicationBuilder()
        => new ApplicationBuilder(ServiceProvider);
}

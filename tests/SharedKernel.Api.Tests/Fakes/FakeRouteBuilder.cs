using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Tests.Fakes;

public sealed class FakeRouteBuilder : IEndpointRouteBuilder
{
    public IServiceProvider ServiceProvider { get; } = new ServiceCollection().BuildServiceProvider();
    public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();
}

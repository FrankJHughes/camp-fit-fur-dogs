using Frank.Registration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions;

[Registration(ServiceLifetime.Singleton)]
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}


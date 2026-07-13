using Frank.Core.Application.Registration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.Endpoints;

[Registration(ServiceLifetime.Singleton)]
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}

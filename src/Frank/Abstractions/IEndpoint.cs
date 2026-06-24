using Microsoft.AspNetCore.Routing;

namespace Frank.Abstractions;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}


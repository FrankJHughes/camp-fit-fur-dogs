using Microsoft.AspNetCore.Routing;

namespace Frank.Api;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}


using Microsoft.AspNetCore.Routing;

namespace SharedKernel.Api;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}


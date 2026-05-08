
using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Hosting;

public interface IHostingProvider
{
    bool IsActive();
    Task ConfigureAsync(WebApplicationBuilder builder);
}

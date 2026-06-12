using Microsoft.AspNetCore.Builder;

namespace Frank.Abstractions.Hosting;

public interface IHostingModule
{
    bool IsActive(WebApplicationBuilder builder);

    Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder);
}

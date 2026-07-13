using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Application.Abstractions.Hosting;

public interface IHostingModule
{
    bool IsActive(WebApplicationBuilder builder);

    Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder);
}

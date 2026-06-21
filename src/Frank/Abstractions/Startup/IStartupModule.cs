using Microsoft.AspNetCore.Builder;

namespace Frank.Abstractions.Startup;

public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}

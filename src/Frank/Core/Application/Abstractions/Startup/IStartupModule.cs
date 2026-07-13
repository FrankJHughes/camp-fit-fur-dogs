using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Application.Abstractions.Startup;

public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}

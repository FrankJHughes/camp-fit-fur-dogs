using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Startup;

public interface IStartupModule
{
    void Add(WebApplicationBuilder builder);
    void Use(WebApplication app);
}

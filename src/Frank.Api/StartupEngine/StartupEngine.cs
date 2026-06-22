using System.Reflection;
using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Builder;

namespace Frank.Api.StartupEngine;

public sealed class StartupEngine
{
    private readonly IReadOnlyList<IStartupModule> _modules;

    public StartupEngine(IEnumerable<IStartupModule> modules)
    {
        _modules = modules
            .OrderBy(m => m.GetType()
                .GetCustomAttribute<StartupModuleAttribute>()?.Order ?? 1000)
            .ToArray();
    }

    public void AddAll(WebApplicationBuilder builder)
    {
        foreach (var module in _modules)
            module.Add(builder);
    }

    public void UseAll(WebApplication app)
    {
        foreach (var module in _modules)
            module.Use(app);
    }
}

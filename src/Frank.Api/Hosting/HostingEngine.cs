using Frank.Abstractions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Frank.Api.Hosting;

public sealed class HostingEngine
{
    private readonly IReadOnlyList<IHostingModule> _modules;

    public HostingEngine(IEnumerable<IHostingModule> modules)
    {
        _modules = modules
            .OrderBy(GetOrder) // <-- NEW: sort by HostingModuleAttribute
            .ToArray();
    }

    private static int GetOrder(IHostingModule module)
    {
        var attr = module.GetType().GetCustomAttributes(typeof(HostingModuleAttribute), false)
                         .Cast<HostingModuleAttribute>()
                         .FirstOrDefault();

        return attr?.Order ?? 0; // default order = 0
    }

    public async Task ApplyHostingEnvironmentConfigurationAsync(WebApplicationBuilder builder)
    {
        var merged = new Dictionary<string, string?>();

        foreach (var module in _modules)
        {
            if (!module.IsActive(builder))
                continue;

            var overrides = await module.GetConfigurationOverridesAsync(builder);
            foreach (var @override in overrides)
                merged[@override.Key] = @override.Value; // later modules win
        }

        if (merged.Count > 0)
        {
            IConfigurationBuilder configBuilder = builder.Configuration;
            configBuilder.Add(new HostingOverridesConfigurationSource(merged));
        }
    }
}

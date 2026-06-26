using Frank.Abstractions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Frank.Api.Hosting;

public sealed class HostingEngine
{
    private readonly IReadOnlyList<IHostingModule> _modules;

    public HostingEngine(IEnumerable<IHostingModule> modules)
    {
        Console.WriteLine("HostingEngine :: Happy birthday!");

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
            Console.WriteLine($"HostingEngine :: Considering {module.GetType().Name}...");

            if (!module.IsActive(builder))
            {
                Console.WriteLine($"HostingEngine :: ...{module.GetType().Name} host is not active.");
                continue;
            }

            var overrides = await module.GetConfigurationOverridesAsync(builder);
            Console.WriteLine("HostingEngine :: {module.GetType().Name} Overrides");
            Console.WriteLine("HostingEngine :: {");
            foreach (var @override in overrides)
            {
                Console.WriteLine($"HostingEngine :: \t[\"{@override.Key}\"] = \"<masked>\",");
                merged[@override.Key] = @override.Value; // later modules win
            }
            Console.WriteLine("HostingEngine :: }");
        }

        if (merged.Count > 0)
        {
            IConfigurationBuilder configBuilder = builder.Configuration;
            configBuilder.Add(new HostingOverridesConfigurationSource(merged));
        }
    }
}

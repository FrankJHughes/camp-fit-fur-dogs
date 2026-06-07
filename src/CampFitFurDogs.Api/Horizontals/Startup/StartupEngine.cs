using System.Reflection;
using Frank.Api.Hosting;

namespace CampFitFurDogs.Api.Horizontals.Startup;

public static class StartupModuleEngine
{
    public static IReadOnlyList<Type> Discover(IConfiguration config)
    {
        return typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<StartupModuleAttribute>() is not null)
            .Select(t => new
            {
                Type = t,
                Order = t.GetCustomAttribute<StartupModuleAttribute>()!.Order
            })
            .OrderBy(x => x.Order)
            .Select(x => x.Type)
            .ToList();
    }

    public static void Validate(IEnumerable<Type> configurators)
    {
        var list = configurators.ToList();

        // Ensure unique order values
        var duplicates = list
            .GroupBy(t => t.GetCustomAttribute<StartupModuleAttribute>()!.Order)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicates.Any())
        {
            var details = string.Join(", ",
                duplicates.Select(g =>
                    $"{g.Key}: {string.Join(", ", g.Select(t => t.Name))}"));

            throw new InvalidOperationException(
                $"Duplicate StartupModuleAttribute.Order values detected: {details}");
        }

        // Ensure both required methods exist
        foreach (var type in list)
        {
            var hasServices = type.GetMethod("Add", BindingFlags.Public | BindingFlags.Static) != null;
            var hasConfigure = type.GetMethod("Use", BindingFlags.Public | BindingFlags.Static) != null;

            if (!hasServices || !hasConfigure)
                throw new InvalidOperationException(
                    $"StartupModule {type.Name} must define both ConfigureServices and Configure.");
        }
    }

    public static void RunAdd(WebApplicationBuilder builder, IEnumerable<Type> configurators)
    {
        // Phase 1 — run configurators' ConfigureServices in order
        foreach (var type in configurators)
        {
            var method = type.GetMethod("Add", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, new object[] { builder.Services, builder.Configuration });
            if (type == typeof(HostingProviderStartupModule))
            {
                // StartupModule 0 - run hosting provider (if registered and active)
                RunHostingProviderIfPresent(builder);
            }
        }
    }

    public static void RunUse(WebApplication app, IEnumerable<Type> configurators)
    {
        foreach (var type in configurators)
        {
            var method = type.GetMethod("Use", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, new object[] { app });
        }
    }

    private static void RunHostingProviderIfPresent(WebApplicationBuilder builder)
    {
        // We build a temporary provider to resolve IHostingProvider.
        // This is safe because it's only used for provider detection/config.
        using var tempProvider = builder.Services.BuildServiceProvider();

        var hostingProvider = tempProvider.GetService<IHostingProvider>();
        if (hostingProvider is null)
            return;

        if (!hostingProvider.IsActive())
            return;

        // Run provider-specific configuration BEFORE the app is built.
        hostingProvider.ConfigureAsync(builder).GetAwaiter().GetResult();
    }
}

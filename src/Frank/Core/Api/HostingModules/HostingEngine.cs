using Frank.Core.Application.Abstractions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Frank.Core.Api.HostingModules;

/// <summary>
/// Coordinates the execution of hosting modules and merges their configuration
/// overrides into the application's configuration pipeline.
/// <para>
/// The hosting engine evaluates each <see cref="IHostingModule"/> in a
/// deterministic order defined by <see cref="HostingModuleAttribute.Order"/>.
/// Modules may activate conditionally based on the hosting environment, and
/// each active module may contribute configuration overrides.
/// </para>
/// <para>
/// Later modules override earlier ones, allowing higher‑priority modules to
/// replace configuration values supplied by lower‑priority modules.
/// </para>
/// </summary>
public sealed class HostingEngine
{
    private readonly IReadOnlyList<IHostingModule> _modules;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostingEngine"/> class,
    /// ordering the provided hosting modules according to their
    /// <see cref="HostingModuleAttribute.Order"/> value.
    /// </summary>
    /// <param name="modules">
    /// A collection of hosting modules to evaluate during hosting initialization.
    /// </param>
    public HostingEngine(IEnumerable<IHostingModule> modules)
    {
        _modules = modules
            .OrderBy(GetOrder)
            .ToArray();
    }

    /// <summary>
    /// Retrieves the ordering value for a hosting module based on its
    /// <see cref="HostingModuleAttribute"/>.
    /// If no attribute is present, the module defaults to order <c>0</c>.
    /// </summary>
    /// <param name="module">The hosting module to inspect.</param>
    /// <returns>The module's configured order, or <c>0</c> if unspecified.</returns>
    private static int GetOrder(IHostingModule module)
    {
        var attr = module.GetType()
                         .GetCustomAttributes(typeof(HostingModuleAttribute), false)
                         .Cast<HostingModuleAttribute>()
                         .FirstOrDefault();

        return attr?.Order ?? 0;
    }

    /// <summary>
    /// Applies hosting‑environment‑specific configuration overrides by evaluating
    /// each hosting module in order and merging their contributions.
    /// <para>
    /// For each module:
    /// <list type="bullet">
    /// <item><description>Checks whether the module is active for the current hosting environment.</description></item>
    /// <item><description>Retrieves configuration overrides via <see cref="IHostingModule.GetConfigurationOverridesAsync"/>.</description></item>
    /// <item><description>Merges overrides into a cumulative dictionary, with later modules winning.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If any overrides are produced, they are added to the application's
    /// configuration pipeline using <see cref="HostingOverridesConfigurationSource"/>.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> whose configuration pipeline will
    /// be augmented with hosting‑module overrides.
    /// </param>
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

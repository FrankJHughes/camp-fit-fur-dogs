using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public sealed class SharedKernelOptions
{
    internal List<Action<IServiceCollection>> InfrastructureRegistrations { get; } = new();

    public List<Assembly> ApiAssemblies { get; } = new();

    public void AddInfrastructureAutoRegistration(
        Assembly[] assemblies,
        Action<AutoRegistrationRuleBuilder> configureRules)
    {
        InfrastructureRegistrations.Add(services =>
        {
            var builder = new AutoRegistrationRuleBuilder();
            configureRules(builder);

            foreach (var assembly in assemblies)
            {
                services.AddAutoRegistration(assembly, builder.Build());
            }
        });
    }

    public void AddEndpointAutoDiscovery(Assembly apiAssembly)
    {
        ApiAssemblies.Add(apiAssembly);
    }
}

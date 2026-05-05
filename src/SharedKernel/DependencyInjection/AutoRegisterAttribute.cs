using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class AutoRegisterAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}

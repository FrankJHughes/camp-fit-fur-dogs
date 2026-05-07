using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class AutoRegisterAttribute(
    ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
    public int MaxRegistrationCount { get; set; } = int.MaxValue;
    public int MinRegistrationCount { get; set; } = 0;
    public bool RegisterConcreteType { get; init; } = false;
}

using Microsoft.Extensions.DependencyInjection;

namespace Frank.Registration;

[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class RegistrationAttribute(
    ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
    public int MaxRegistrationCount { get; set; } = int.MaxValue;
    public int MinRegistrationCount { get; set; } = 0;
    public bool RegisterConcreteType { get; init; } = false;
}

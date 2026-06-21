using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Time;

[AutoRegister(ServiceLifetime.Singleton, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

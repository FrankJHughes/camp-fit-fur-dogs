using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface ICurrentUser
{
    Guid Id { get; }
}

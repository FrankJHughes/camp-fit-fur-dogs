using Microsoft.Extensions.DependencyInjection;
using Frank.DependencyInjection;

namespace Frank.Abstractions;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface ICurrentUserService
{
    Guid CurrentUserId { get; }
}

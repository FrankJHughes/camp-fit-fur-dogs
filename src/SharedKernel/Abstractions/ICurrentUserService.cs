using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Abstractions;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface ICurrentUserService
{
    Guid CurrentUserId { get; }
}

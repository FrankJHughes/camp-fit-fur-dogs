using Microsoft.Extensions.DependencyInjection;
using Frank.DependencyInjection;

namespace Frank.Abstractions;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}

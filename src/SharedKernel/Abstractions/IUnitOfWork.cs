using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Abstractions;

[AutoRegister(ServiceLifetime.Scoped)]
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}

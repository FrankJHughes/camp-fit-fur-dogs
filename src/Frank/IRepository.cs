using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

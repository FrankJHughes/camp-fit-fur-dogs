using Microsoft.Extensions.DependencyInjection;

using SharedKernel.DependencyInjection;

namespace CampFitFurDogs.Domain.Customers;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ICustomerRepository
{
    Task<bool> EmailExistsAsync(Email email, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
}

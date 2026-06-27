using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Domain.Customers;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);
}

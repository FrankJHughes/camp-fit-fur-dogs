using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Domain.Customers;

[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);
}

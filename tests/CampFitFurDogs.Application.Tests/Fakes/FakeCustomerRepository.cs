using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeCustomerRepository : ICustomerRepository
{
    public List<Customer> Customers { get; } = [];

    public Task<bool> EmailExistsAsync(Email email, CancellationToken ct)
    {
        return Task.FromResult(Customers.Any(c => c.Email.Equals(email)));
    }

    public Task AddAsync(Customer customer, CancellationToken ct)
    {
        Customers.Add(customer);
        return Task.CompletedTask;
    }
}

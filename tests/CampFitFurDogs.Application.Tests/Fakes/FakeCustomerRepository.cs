using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeCustomerRepository : ICustomerRepository
{
    public List<Domain.Customers.Customer> Customers { get; } = [];

    public int AddCallCount { get; private set; }

    public System.Exception? ExceptionToThrow { get; set; }

    public Task<bool> EmailExistsAsync(Email email, CancellationToken ct)
    {
        return Task.FromResult(Customers.Any(c => c.Email.Equals(email)));
    }

    public Task AddAsync(Domain.Customers.Customer customer, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        Customers.Add(customer);
        AddCallCount++;

        return Task.CompletedTask;
    }
}

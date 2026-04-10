namespace CampFitFurDogs.Domain.Customers;

public interface ICustomerRepository
{
    Task<bool> EmailExistsAsync(Email email, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
}

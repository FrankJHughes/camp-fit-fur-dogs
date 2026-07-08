namespace CampFitFurDogs.Application.Abstractions.Customer.GetCustomerById;

public interface IGetCustomerByIdReader
{
    Task<GetCustomerByIdResponse?> GetByIdAsync(
        Guid customerId, CancellationToken cancellationToken);
}

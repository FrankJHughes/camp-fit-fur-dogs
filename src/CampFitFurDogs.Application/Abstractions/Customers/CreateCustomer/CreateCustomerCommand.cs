
using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password) : ICommand<Guid>;

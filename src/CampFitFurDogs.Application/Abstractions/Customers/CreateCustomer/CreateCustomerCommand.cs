
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string ExternalId,
    string? Phone = null
) : ICommand<Guid>;

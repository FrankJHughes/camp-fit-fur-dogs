using Frank.Abstractions.Command;

namespace CampFitFurDogs.Application.Abstractions.Customer.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string ExternalId,
    string? Phone = null
) : ICommand<Guid>;

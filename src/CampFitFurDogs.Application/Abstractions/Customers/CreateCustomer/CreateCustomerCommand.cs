
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone = null,
    string? Password = null,
    string? ExternalAuthProviderId = null) : ICommand<Guid>;

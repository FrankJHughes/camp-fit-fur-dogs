
using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password) : ICommand<Guid>;

namespace CampFitFurDogs.Application.Abstractions.Customer.GetCustomerById;

public record GetCustomerByIdResponse
(
    Guid Id,
    string FirstName,
    string LastName
);

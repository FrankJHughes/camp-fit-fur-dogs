namespace CampFitFurDogs.Api.Customers;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password);

using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Customers.CreateCustomer;

public sealed class CreateCustomerHandler
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _repo;

    public CreateCustomerHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var email = Email.From(request.Email);

        if (await _repo.EmailExistsAsync(email, ct))
            throw new EmailAlreadyExistsException(email.Value);

        var phone = PhoneNumber.From(request.Phone);
        var passwordHash = PasswordHash.From(HashPassword(request.Password));

        var customer = Customer.Create(
            request.FirstName,
            request.LastName,
            email,
            phone,
            passwordHash);

        await _repo.AddAsync(customer, ct);

        return customer.Id.Value;
    }

    private static string HashPassword(string raw)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(raw));
    }
}

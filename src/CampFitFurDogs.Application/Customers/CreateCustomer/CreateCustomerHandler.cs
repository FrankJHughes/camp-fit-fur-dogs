// src/CampFitFurDogs.Application/Customers/CreateCustomer/CreateCustomerHandler.cs
using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Customers.CreateCustomer;

public sealed class CreateCustomerHandler
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(ICustomerRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateCustomerCommand request, CancellationToken ct)
    {
        var email = Email.From(request.Email);
        if (await _repo.EmailExistsAsync(email, ct))
            throw new EmailAlreadyExistsException(email.Value);

        var phone = PhoneNumber.From(request.Phone);
        var passwordHash = PasswordHash.Create(request.Password);

        var customer = Customer.Create(
            request.FirstName,
            request.LastName,
            email,
            phone,
            passwordHash);

        await _repo.AddAsync(customer, ct);
        await _unitOfWork.CommitAsync(ct);

        return customer.Id.Value;
    }
}

using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Abstractions;

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
        ct.ThrowIfCancellationRequested();

        // Convert primitives → Value Objects
        var firstName = FirstName.From(request.FirstName);
        var lastName = LastName.From(request.LastName);
        var email = Email.From(request.Email);
        var externalId = ExternalId.From(request.ExternalId);

        var phone = string.IsNullOrWhiteSpace(request.Phone) ? null : PhoneNumber.From(request.Phone);

        // Create domain entity (domain enforces identity invariants)
        var customer = Customer.Create(
            firstName: firstName,
            lastName: lastName,
            email: email,
            externalId: externalId,
            phone: phone
        );

        // Persist
        await _repo.AddAsync(customer, ct);
        await _unitOfWork.CommitAsync(ct);

        return customer.Id.Value;
    }
}

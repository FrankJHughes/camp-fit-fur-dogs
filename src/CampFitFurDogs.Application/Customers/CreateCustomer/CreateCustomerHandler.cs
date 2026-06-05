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

        PhoneNumber? phone =
            string.IsNullOrWhiteSpace(request.Phone)
                ? null
                : PhoneNumber.From(request.Phone);

        // Duplicate email check applies ONLY to local accounts
        if (request.Password is not null)
        {
            if (await _repo.EmailExistsAsync(email, ct))
                throw new EmailAlreadyExistsException(email.Value);
        }

        // Identity source is validated by:
        // - Command validator (semantic)
        // - Domain (invariant)
        var passwordHash =
            request.Password is not null
                ? PasswordHash.From(request.Password)
                : null;

        var externalId =
            request.ExternalAuthProviderId is not null
                ? ExternalAuthProviderId.From(request.ExternalAuthProviderId)
                : null;

        // Create domain entity (domain enforces identity invariants)
        var customer = Customer.Create(
            firstName,
            lastName,
            email,
            phone,
            passwordHash,
            externalId);

        // Persist
        await _repo.AddAsync(customer, ct);
        await _unitOfWork.CommitAsync(ct);

        return customer.Id.Value;
    }
}

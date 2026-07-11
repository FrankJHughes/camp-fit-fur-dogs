using Frank.Application.Abstractions.Users.CreateUser;
using Frank.Domain.Users;
using Frank.Abstractions.Command;
using Frank.Abstractions.UnitOfWork;

namespace Frank.Application.Users.CreateUser;

public sealed class CreateUserCommandHandler
    : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _repo;
    private readonly IFrankIdentityUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository repo, IFrankIdentityUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateUserCommand request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // Convert primitives → Value Objects
        var firstName = FirstName.From(request.FirstName);
        var lastName = LastName.From(request.LastName);
        var email = Email.From(request.Email);
        var externalId = ExternalId.From(request.ExternalId);

        var phone = string.IsNullOrWhiteSpace(request.Phone) ? null : PhoneNumber.From(request.Phone);

        // Create domain entity (domain enforces identity invariants)
        var user = User.Create(
            firstName: firstName,
            lastName: lastName,
            email: email,
            externalId: externalId,
            phone: phone
        );

        // Persist
        await _repo.AddAsync(user, ct);
        await _unitOfWork.CommitAsync(ct);

        return user.Id.Value;
    }
}

using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Users.CreateUser;

public sealed class CreateUserCommandHandler
    : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly ICreateUserWriter _writer;
    private readonly IFrankIdentityUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(ICreateUserWriter writer, IFrankIdentityUnitOfWork unitOfWork)
    {
        _writer = writer;
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
        await _writer.WriteAsync(user, ct);
        await _unitOfWork.CommitAsync(ct);

        return user.Id.Value;
    }
}

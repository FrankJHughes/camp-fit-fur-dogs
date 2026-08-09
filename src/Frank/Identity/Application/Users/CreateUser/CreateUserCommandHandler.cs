using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Users.CreateUser;

/// <summary>
/// Handles a <see cref="CreateUserCommand"/> by constructing a new
/// <see cref="User"/> domain entity from the provided primitives and persisting
/// it through the configured writer and unit of work.
/// <para>
/// This command handler is responsible for user creation during onboarding,
/// external‑identity provisioning, or administrative user management.
/// </para>
/// <para>
/// The handler converts all incoming primitive values into strongly‑typed
/// domain value objects, ensuring that domain invariants are enforced at the
/// boundary.
/// </para>
/// </summary>
public sealed class CreateUserCommandHandler
    : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly ICreateUserWriter _writer;
    private readonly IFrankIdentityUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new <see cref="CreateUserCommandHandler"/> using the provided
    /// user writer and unit of work.
    /// </summary>
    /// <param name="writer">
    /// The writer responsible for persisting newly created <see cref="User"/>
    /// entities.
    /// </param>
    /// <param name="unitOfWork">
    /// The unit of work used to commit the user creation operation to the
    /// underlying persistence store.
    /// </param>
    public CreateUserCommandHandler(ICreateUserWriter writer, IFrankIdentityUnitOfWork unitOfWork)
    {
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Executes the command by converting the incoming primitives into domain
    /// value objects, constructing a new <see cref="User"/> entity, persisting
    /// it, and committing the unit of work.
    /// </summary>
    /// <param name="request">
    /// The command containing the user‑creation data such as first name, last
    /// name, email, external identity, and optional phone number.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The <see cref="Guid"/> identifier of the newly created user.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the cancellation token is signaled before completion.
    /// </exception>
    public async Task<Guid> HandleAsync(CreateUserCommand request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // Convert primitives → Value Objects
        var firstName = FirstName.From(request.FirstName);
        var lastName = LastName.From(request.LastName);
        var email = Email.From(request.Email);
        var externalId = ExternalId.From(request.ExternalId);

        var phone = string.IsNullOrWhiteSpace(request.Phone)
            ? null
            : PhoneNumber.From(request.Phone);

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

using Frank.Abstractions.Command;

namespace Frank.Application.Abstractions.Users.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string ExternalId,
    string? Phone = null
) : ICommand<Guid>;

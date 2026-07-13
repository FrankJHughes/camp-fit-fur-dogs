using Frank.Core.Application.Abstractions.Command;

namespace Frank.Identity.Application.Abstractions.Users.CreateUser;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string ExternalId,
    string? Phone = null
) : ICommand<Guid>;

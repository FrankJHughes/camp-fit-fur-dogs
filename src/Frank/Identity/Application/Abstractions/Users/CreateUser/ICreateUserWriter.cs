using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Abstractions.Users.CreateUser;

public interface ICreateUserWriter
{
    Task WriteAsync(
        User user, CancellationToken cancellationToken);
}

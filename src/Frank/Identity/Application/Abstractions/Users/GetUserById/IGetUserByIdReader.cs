namespace Frank.Identity.Application.Abstractions.Users.GetUserById;

public interface IGetUserByIdReader
{
    Task<GetUserByIdResponse?> ReadAsync(
        Guid UserId, CancellationToken cancellationToken);
}

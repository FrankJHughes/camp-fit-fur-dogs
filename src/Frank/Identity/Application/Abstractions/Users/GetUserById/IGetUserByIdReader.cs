namespace Frank.Identity.Application.Abstractions.Users.GetUserById;

public interface IGetUserByIdReader
{
    Task<GetUserByIdResponse?> GetByIdAsync(
        Guid UserId, CancellationToken cancellationToken);
}

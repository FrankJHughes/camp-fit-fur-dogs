namespace Frank.Identity.Domain.Users;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct);
}

using Frank.Identity.Domain.Users;

namespace Frank.TestUtilities.Fakes;

public class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = [];

    public int AddCallCount { get; private set; }

    public System.Exception? ExceptionToThrow { get; set; }

    public Task<bool> EmailExistsAsync(Email email, CancellationToken ct)
    {
        return Task.FromResult(Users.Any(c => c.Email.Equals(email)));
    }

    public Task AddAsync(User user, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        Users.Add(user);
        AddCallCount++;

        return Task.CompletedTask;
    }
}

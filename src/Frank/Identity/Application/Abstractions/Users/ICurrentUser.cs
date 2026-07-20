namespace Frank.Identity.Application.Abstractions.Users;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Name { get; }
}

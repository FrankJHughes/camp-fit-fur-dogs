namespace Frank.Abstractions.Identity;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Name { get; }
}

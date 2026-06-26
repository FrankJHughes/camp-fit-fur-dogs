namespace Frank.Abstractions.Identity;

public interface ICurrentUser
{
    Guid? Id { get; }
    bool IsAuthenticated { get; }
}

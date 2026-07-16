namespace Frank.Identity.Application.Abstractions;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Name { get; }
}

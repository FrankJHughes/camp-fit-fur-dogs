namespace Frank.Identity.Application.Abstractions.Users.FindUserByExternalId;

public interface IFindUserByExternalIdReader
{
    Task<FindUserByExternalIdResponse?> FindByExternalIdAsync(
        string externalId, CancellationToken cancellationToken);
}

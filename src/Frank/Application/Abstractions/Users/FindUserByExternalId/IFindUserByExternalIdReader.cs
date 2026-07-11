namespace Frank.Application.Abstractions.Users.FindUserByExternalId;

public interface IFindUserByExternalIdReader
{
    Task<FindUsererByExternalIdResponse?> FindByExternalIdAsync(
        string externalId, CancellationToken cancellationToken);
}

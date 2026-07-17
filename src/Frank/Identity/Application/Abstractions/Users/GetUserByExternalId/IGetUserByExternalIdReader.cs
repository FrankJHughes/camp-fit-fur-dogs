namespace Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;

public interface IGetUserByExternalIdReader
{
    Task<GetUserByExternalIdResponse?> GetByExternalIdAsync(
        string externalId, CancellationToken cancellationToken);
}

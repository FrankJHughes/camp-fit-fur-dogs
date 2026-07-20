namespace Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;

public interface IGetUserByExternalIdReader
{
    Task<GetUserByExternalIdResponse?> ReadAsync(
        string externalId, CancellationToken cancellationToken);
}

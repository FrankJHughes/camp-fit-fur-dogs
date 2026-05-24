namespace CampFitFurDogs.Application.Abstractions.Identity.External;

public interface IExternalIdentityResolver
{
    Task<Guid> ResolveAsync(
        string externalUserId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken);
}

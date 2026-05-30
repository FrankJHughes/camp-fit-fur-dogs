namespace CampFitFurDogs.Application.Abstractions.Authentication;

public interface IAuthCallbackService
{
    Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct);
}

using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public interface IAuthCallbackStep
{
    Task ExecuteAsync(AuthCallbackContext context, CancellationToken ct);
}

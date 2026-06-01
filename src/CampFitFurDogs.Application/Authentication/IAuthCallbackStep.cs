
namespace CampFitFurDogs.Application.Authentication;

public interface IAuthCallbackStep
{
    Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct);
}

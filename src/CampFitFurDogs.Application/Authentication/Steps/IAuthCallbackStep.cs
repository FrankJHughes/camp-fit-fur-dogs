using System.Threading;
using System.Threading.Tasks;

namespace CampFitFurDogs.Application.Authentication.Steps;

public interface IAuthCallbackStep
{
    Task ExecuteAsync(AuthCallbackContext context, CancellationToken ct);
}

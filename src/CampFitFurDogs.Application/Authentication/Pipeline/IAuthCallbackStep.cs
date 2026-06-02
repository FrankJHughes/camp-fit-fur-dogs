namespace CampFitFurDogs.Application.Authentication.Pipeline;

public interface IAuthCallbackStep
{
    /// <summary>
    /// Metadata describing this step for diagnostics, tracing, and logs.
    /// </summary>
    AuthCallbackStepMetadata Metadata { get; }

    /// <summary>
    /// Executes this step and returns a new immutable context.
    /// </summary>
    Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct);
}

using Frank.Abstractions.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observations;

/// <summary>
/// Observability context for request-scope operations.
/// Includes authenticated user identity when available.
/// </summary>
public sealed class DefaultRequestObservationContext : ObservationContextBase, IRequestObservationContext
{
    public string? UserId { get; }

    public DefaultRequestObservationContext(IHostEnvironment hostEnvironment)
        : base("none", "none", "none", hostEnvironment, new Dictionary<string, object?>())
    {

    }

}

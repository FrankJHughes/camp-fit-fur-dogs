using Frank.Abstractions.Observability;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observability;

/// <summary>
/// Observability context for request-scope operations.
/// Includes authenticated user identity when available.
/// </summary>
public sealed class DefaultRequestObservabilityContext : ObservabilityContextBase, IRequestObservabilityContext
{
    public string? UserId { get; }

    public DefaultRequestObservabilityContext(IHostEnvironment hostEnvironment)
        : base("none", "none", "none", hostEnvironment, new Dictionary<string, object?>())
    {

    }

}

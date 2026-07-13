using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

public sealed class CorrelationContext : ICorrelationContext
{
    public string Create()
        => Guid.NewGuid().ToString("N");

    public string Propagate(string? incoming)
        => string.IsNullOrWhiteSpace(incoming)
            ? Create()
            : incoming;
}

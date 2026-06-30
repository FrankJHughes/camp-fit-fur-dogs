// src/Frank/Abstractions/Observability/ICorrelationContext.cs
namespace Frank.Abstractions.Observations;

/// <summary>
/// Responsible for creating and propagating correlation identifiers.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Creates a new correlation identifier.
    /// </summary>
    string Create();

    /// <summary>
    /// Propagates an incoming correlation identifier or creates a new one if missing.
    /// </summary>
    string Propagate(string? incoming);
}

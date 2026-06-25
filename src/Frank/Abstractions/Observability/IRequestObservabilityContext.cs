// src/Frank/Abstractions/Observability/IObservabilityContext.cs
namespace Frank.Abstractions.Observability;

/// <summary>
/// Represents the structured context that flows through all observable operations.
/// This context is immutable and must be provided by the infrastructure layer.
/// </summary>
public interface IRequestObservabilityContext : IObservabilityContext
{
    string? UserId { get; }
}

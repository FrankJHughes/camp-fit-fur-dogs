// src/Frank/Abstractions/Observability/IObservabilityContext.cs
namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Represents the structured context that flows through all observable operations.
/// This context is immutable and must be provided by the infrastructure layer.
/// </summary>
public interface IRequestObservationContext : IObservationContext
{
    string? UserId { get; }
}

namespace Frank.Core.Domain;

/// <summary>
/// Marker interface for domain events.
///
/// Domain events represent significant occurrences within the domain model.
/// They are raised by aggregate roots and consumed by the application layer,
/// which is responsible for dispatching, logging, and handling them.
///
/// Domain events must be immutable and should contain only domain-relevant data.
/// </summary>
public interface IDomainEvent { }

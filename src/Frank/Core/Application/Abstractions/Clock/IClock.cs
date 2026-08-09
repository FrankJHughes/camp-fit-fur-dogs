namespace Frank.Core.Application.Abstractions.Clock;

/// <summary>
/// Provides access to the current UTC time in a testable and deterministic way.
///
/// <para>
/// The <see cref="IClock"/> abstraction allows application code to obtain the
/// current time without directly depending on <see cref="DateTimeOffset.UtcNow"/>.
/// This enables reliable unit testing, time‑based logic simulation, and
/// consistent behavior across environments.
/// </para>
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current time expressed as a <see cref="DateTimeOffset"/> in UTC.
    ///
    /// <para>
    /// Implementations may return the system clock, a fixed time, or a
    /// controlled/virtual time source depending on the application's needs.
    /// </para>
    /// </summary>
    DateTimeOffset UtcNow { get; }
}

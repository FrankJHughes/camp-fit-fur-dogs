using Frank.Core.Application.Abstractions.Clock;

namespace Frank.Core.Infrastructure.Clock;

/// <summary>
/// Provides the system‑based implementation of <see cref="IClock"/>,
/// returning the current UTC time using <see cref="DateTimeOffset.UtcNow"/>.
/// <para>
/// This implementation is suitable for production scenarios where the system
/// clock is the authoritative source of time. For testing or deterministic
/// workflows, a custom or fixed clock implementation can be substituted.
/// </para>
/// </summary>
public sealed class SystemClock : IClock
{
    /// <summary>
    /// Gets the current UTC timestamp from the system clock.
    /// </summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

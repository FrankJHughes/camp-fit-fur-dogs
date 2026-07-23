
using Frank.Core.Application.Abstractions.Clock;

namespace Frank.Core.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

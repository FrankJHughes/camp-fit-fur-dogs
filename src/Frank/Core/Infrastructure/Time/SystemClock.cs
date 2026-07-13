
using Frank.Core.Application.Abstractions.Time;

namespace Frank.Core.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

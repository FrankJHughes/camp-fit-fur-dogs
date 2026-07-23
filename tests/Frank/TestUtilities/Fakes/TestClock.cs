using Frank.Core.Application.Abstractions.Clock;

namespace Frank.TestUtilities.Fakes;

public sealed class TestClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
}

using Frank.Core.Application.Abstractions.Time;

namespace Frank.TestUtilities.Fakes;

public sealed class TestClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
}

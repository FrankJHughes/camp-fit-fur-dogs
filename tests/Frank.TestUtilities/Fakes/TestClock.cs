using System;
using Frank.Abstractions.Time;

namespace Frank.TestUtilities.Fakes;

public sealed class TestClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
}

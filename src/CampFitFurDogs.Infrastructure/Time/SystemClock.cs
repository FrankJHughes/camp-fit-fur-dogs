
using Frank.Abstractions.Time;

namespace CampFitFurDogs.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

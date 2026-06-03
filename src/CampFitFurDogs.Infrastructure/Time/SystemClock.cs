
using CampFitFurDogs.Application.Abstractions.Time;

namespace CampFitFurDogs.Infrastructure.Time;

public sealed class SystemClock : ISystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

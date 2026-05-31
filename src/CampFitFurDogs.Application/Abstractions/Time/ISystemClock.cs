namespace CampFitFurDogs.Application.Abstractions.Time;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
}

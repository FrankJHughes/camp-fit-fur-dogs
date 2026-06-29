namespace Frank.Abstractions.Time;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

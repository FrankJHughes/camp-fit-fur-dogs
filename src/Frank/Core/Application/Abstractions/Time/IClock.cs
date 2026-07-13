namespace Frank.Core.Application.Abstractions.Time;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

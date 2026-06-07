using Microsoft.Extensions.Logging;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class TestLogger<T> : ILogger<T>, IDisposable
{
    public List<string> Messages { get; } = new();

    // Explicit interface implementation avoids nullability constraint mismatches
    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return this;
    }

    public void Dispose() { }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Messages.Add(formatter(state, exception));
    }
}

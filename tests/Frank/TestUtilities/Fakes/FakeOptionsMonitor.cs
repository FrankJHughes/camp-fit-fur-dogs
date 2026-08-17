using Microsoft.Extensions.Options;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeOptionsMonitor<T> : IOptionsMonitor<T>
{
    private readonly T _value;

    public FakeOptionsMonitor(T value)
    {
        _value = value;
    }

    public T CurrentValue => _value;

    public T Get(string? name) => _value;

    public IDisposable OnChange(Action<T, string?> listener)
        => NullDisposable.Instance;

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new();

        public void Dispose()
        {
            // no-op
        }
    }
}

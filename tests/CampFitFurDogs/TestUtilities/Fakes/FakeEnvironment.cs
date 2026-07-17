using Frank.Core.Application.Abstractions.EnvironmentVariables;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeEnvironment : IEnvironmentVariables
{
    private readonly Dictionary<string, string?> _values = [];

    public void Set(string key, string? value)
        => _values[key] = value;

    public string? Get(string key)
        => _values.TryGetValue(key, out var value) ? value : null;
}

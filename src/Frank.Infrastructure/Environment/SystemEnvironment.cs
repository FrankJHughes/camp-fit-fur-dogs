using Frank.Abstractions.Environment;

namespace Frank.Infrastructure.Environment;

public sealed class SystemEnvironment : IEnvironment
{
    public string? Get(string key) => System.Environment.GetEnvironmentVariable(key);
}

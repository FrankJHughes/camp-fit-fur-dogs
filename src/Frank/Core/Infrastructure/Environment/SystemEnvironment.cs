using Frank.Core.Application.Abstractions.Environment;

namespace Frank.Core.Infrastructure.Environment;

public sealed class SystemEnvironment : IEnvironment
{
    public string? Get(string key) => System.Environment.GetEnvironmentVariable(key);
}

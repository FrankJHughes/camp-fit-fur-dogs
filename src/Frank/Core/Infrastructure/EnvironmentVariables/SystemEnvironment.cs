using Frank.Core.Application.Abstractions.EnvironmentVariables;

namespace Frank.Core.Infrastructure.EnvironmentVariables;

public sealed class SystemEnvironmentVariables : IEnvironmentVariables
{
    public string? Get(string key) => System.Environment.GetEnvironmentVariable(key);
}

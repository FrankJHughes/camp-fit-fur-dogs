namespace Frank.Core.Application.Abstractions.EnvironmentVariables;

public interface IEnvironmentVariables
{
    string? Get(string key);
}

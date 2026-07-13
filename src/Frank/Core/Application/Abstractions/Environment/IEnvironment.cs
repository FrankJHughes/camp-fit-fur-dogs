namespace Frank.Core.Application.Abstractions.Environment;

public interface IEnvironment
{
    string? Get(string key);
}

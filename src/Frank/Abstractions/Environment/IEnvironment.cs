using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Environment;

[AutoRegister(ServiceLifetime.Singleton)]
public interface IEnvironment
{
    string? Get(string key);
}

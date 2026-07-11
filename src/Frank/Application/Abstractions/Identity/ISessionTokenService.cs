using Frank.Domain.Sessions;

namespace Frank.Application.Abstractions.Identity;

public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash Hash
);

public interface ISessionTokenService
{
    GeneratedSessionToken Generate();
}

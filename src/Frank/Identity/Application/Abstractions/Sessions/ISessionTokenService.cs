using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions;

public interface ISessionTokenService
{
    GeneratedSessionToken Generate();
    SessionTokenHash Hash(string plaintextToken);
}

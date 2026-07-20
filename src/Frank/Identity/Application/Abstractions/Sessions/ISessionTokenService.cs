using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions;

public interface ISessionTokenGenerator
{
    GeneratedSessionToken Generate();
    SessionTokenHash Hash(string plaintextToken);
}

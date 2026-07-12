using Frank.Application.Abstractions.Identity;
using Frank.Domain.Sessions;

namespace Frank.Application.Abstractions.Authentication;

public interface ISessionTokenService
{
    GeneratedSessionToken Generate();
    SessionTokenHash Hash(string plaintextToken);
}

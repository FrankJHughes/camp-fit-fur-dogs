using Frank.Identity.Application.Abstractions;
using Frank.Identity.Domain.Sessions;

namespace Frank.Core.Application.Abstractions.Authentication;

public interface ISessionTokenService
{
    GeneratedSessionToken Generate();
    SessionTokenHash Hash(string plaintextToken);
}

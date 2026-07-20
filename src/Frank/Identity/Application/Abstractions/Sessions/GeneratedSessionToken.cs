using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions;

public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash HashedToken
);

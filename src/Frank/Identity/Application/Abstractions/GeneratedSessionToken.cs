using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions;

public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash HashedToken
);

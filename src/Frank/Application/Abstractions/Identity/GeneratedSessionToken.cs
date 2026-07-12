using Frank.Domain.Sessions;

namespace Frank.Application.Abstractions.Identity;

public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash HashedToken
);

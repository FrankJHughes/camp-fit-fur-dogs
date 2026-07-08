using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash HashedToken
);

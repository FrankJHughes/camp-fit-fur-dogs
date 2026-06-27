using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed record GeneratedSessionToken(
    string PlaintextToken,
    SessionTokenHash Hash
);

public interface ISessionTokenService
{
    GeneratedSessionToken Generate();
}

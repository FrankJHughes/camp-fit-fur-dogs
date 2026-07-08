using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.Application.Abstractions.Authentication;

public interface ISessionTokenService
{
    GeneratedSessionToken Generate();
    SessionTokenHash Hash(string plaintextToken);
}

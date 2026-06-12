namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthHandlerClaims
{
    public Dictionary<string, string> Claims { get; }

    public FakeAuthHandlerClaims(Dictionary<string, string> claims)
    {
        Claims = claims;
    }
}

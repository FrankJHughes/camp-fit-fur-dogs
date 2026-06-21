using Frank.Abstractions.Authentication.Callback;

namespace CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;

public static class FakeFrankAuthCallbackResult
{
    public static FrankAuthCallbackResult Create(string subjectId = "sub-123")
        => new()
        {
            SubjectId = subjectId,
            GivenName = "Test",
            FamilyName = "User",
            Email = "test@example.com",
            Claims = new Dictionary<string, string> { ["sub"] = subjectId }
        };
}

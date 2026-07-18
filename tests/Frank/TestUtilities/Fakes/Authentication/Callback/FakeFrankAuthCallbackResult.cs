using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.TestUtilities.Fakes.Authentication.Callback;

public static class FakeOidcCallbackResult
{
    public static CallbackOidcContextBuilderResult Create(string subjectId = "sub-123")
        => new()
        {
            SubjectId = subjectId,
            GivenName = "Test",
            FamilyName = "User",
            Email = "test@example.com",
            Claims = new Dictionary<string, string> { ["sub"] = subjectId }
        };
}

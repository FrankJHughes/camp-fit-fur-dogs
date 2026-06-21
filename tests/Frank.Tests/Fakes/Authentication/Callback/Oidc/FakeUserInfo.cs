using Frank.Authentication.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc;

public static class FakeUserInfo
{
    public static OidcUserInfo Basic =>
        new(
            Subject: "user-123",
            Email: "test@campfitfurdogs.com",
            GivenName: "Test",
            FamilyName: "User",
            Picture: null,
            Claims: new Dictionary<string, string>
            {
                ["sub"] = "user-123",
                ["email"] = "test@campfitfurdogs.com",
                ["given_name"] = "Test",
                ["family_name"] = "User"
            }
        );
}

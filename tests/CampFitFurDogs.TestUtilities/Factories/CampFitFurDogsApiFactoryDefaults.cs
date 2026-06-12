using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class CampFitFurDogsApiFactoryDefaults
{
    public static CampFitFurDogsApiFactory WithDefaultApiConfig(
        this CampFitFurDogsApiFactory factory)
    {
        return factory
            .WithFrontendBaseUrl("http://localhost:3000")
            .WithConfigOverrides(cfg =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Oidc:Authority"] = "test.example.com",
                    ["Authentication:Oidc:ClientId"] = "test-client-id",
                    ["Authentication:Oidc:ClientSecret"] = "test-secret",
                    ["Authentication:Oidc:CallbackUrl"] = "https://localhost/api/auth/callback",
                    ["Authentication:Oidc:PostLoginRedirectUrl"] = "https://localhost:3000/"
                });
            });
    }
}

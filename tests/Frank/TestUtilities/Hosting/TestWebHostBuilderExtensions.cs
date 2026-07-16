using Microsoft.AspNetCore.Hosting;

namespace Frank.TestUtilities.Hosting;

public static class TestWebHostBuilderExtensions
{
    public static IWebHostBuilder UseTestServices(this IWebHostBuilder builder)
    {
        return builder.ConfigureServices(services =>
        {
            // Example overrides:
            // services.AddSingleton<IClock, TestClock>();
            // services.AddAuthentication("Test")
            //     .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", _ => { });
        });
    }
}

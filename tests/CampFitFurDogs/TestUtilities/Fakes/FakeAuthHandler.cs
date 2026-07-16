using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "FakeAuth";

    public FakeAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claimSource = Context.RequestServices.GetService<FakeAuthHandlerClaims>();

        // Pull claims from the test builder if provided
        var claims = claimSource?.Claims
            .Select(kvp => new Claim(kvp.Key, kvp.Value))
            .ToList()
            ?? new List<Claim>();

        // Ensure we have a user ID
        var userId =
            claims.FirstOrDefault(c => c.Type == "sub")?.Value
            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            ?? Guid.NewGuid().ToString(); // fallback for safety

        // Guarantee both sub and NameIdentifier exist and match
        if (!claims.Any(c => c.Type == "sub"))
            claims.Add(new Claim("sub", userId));

        if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

        // Ensure Name exists (not required, but nice for debugging)
        if (!claims.Any(c => c.Type == ClaimTypes.Name))
            claims.Add(new Claim(ClaimTypes.Name, "Fake Test User"));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

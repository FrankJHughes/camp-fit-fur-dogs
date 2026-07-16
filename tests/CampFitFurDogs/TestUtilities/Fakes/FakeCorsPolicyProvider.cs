using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeCorsPolicyProvider : ICorsPolicyProvider
{
    private readonly CorsPolicy _policy;

    public FakeCorsPolicyProvider(CorsPolicy policy)
    {
        _policy = policy;
    }

    public Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName)
    {
        return Task.FromResult<CorsPolicy?>(_policy);
    }
}

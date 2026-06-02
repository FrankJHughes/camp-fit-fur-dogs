using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Application.Authentication.Pipeline;
using CampFitFurDogs.Application.Authentication.Pipeline.Steps;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.TestUtilities.Fakes;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackPipelineDiscoveryTests
{
    // ------------------------------------------------------------
    // Fakes required for pipeline step dependencies
    // ------------------------------------------------------------

    private sealed class FakeAuthClient : IAuthClient
    {
        public Task<AuthToken> ExchangeAsync(string code, CancellationToken ct)
            => Task.FromResult(new AuthToken("fake_access_token"));

        public Task<AuthUser> GetUserAsync(AuthToken token, CancellationToken ct)
            => Task.FromResult(new AuthUser(
                ExternalId: "fake-sub",
                GivenName: "Fake",
                FamilyName: "User",
                Email: "fake@example.com"
            ));
    }

    private sealed class FakeIdentityResolver : IIdentityResolver
    {
        public Task<Guid> ResolveAsync(AuthUser user, CancellationToken ct)
            => Task.FromResult<Guid>(Guid.NewGuid());
    }

    private sealed class FakeSessionRepository : ISessionRepository
    {
        public Task CreateAsync(Session session)
            => Task.CompletedTask;

        public Task RevokeAsync(SessionTokenHash tokenHash)
            => Task.CompletedTask;

        public Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash)
            => Task.FromResult<Session?>(null);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);
    }

    // ------------------------------------------------------------
    // DI Setup for Discovery Tests
    // ------------------------------------------------------------

    private ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        // Register all pipeline steps
        services.AddScoped<IAuthCallbackStep, ValidateConfigurationStep>();
        services.AddScoped<IAuthCallbackStep, ExchangeCodeStep>();
        services.AddScoped<IAuthCallbackStep, FetchUserStep>();
        services.AddScoped<IAuthCallbackStep, ValidateUserStep>();
        services.AddScoped<IAuthCallbackStep, ResolveIdentityStep>();
        services.AddScoped<IAuthCallbackStep, IssueCookieStep>();
        services.AddScoped<IAuthCallbackStep, CreateSessionStep>();
        services.AddScoped<IAuthCallbackStep, AuditLoginStep>();
        services.AddScoped<IAuthCallbackStep, BuildRedirectStep>();

        // Required dependencies for steps
        services.AddSingleton<IAuthClient, FakeAuthClient>();
        services.AddSingleton<IIdentityResolver, FakeIdentityResolver>();
        services.AddSingleton<IAuditLogger, FakeAuditLogger>();
        services.AddSingleton<ISessionTokenService, FakeTokenService>();
        services.AddSingleton<ISessionRepository, FakeSessionRepository>();
        services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();

        // Options required by ValidateConfigurationStep
        services.AddOptions<OidcOptions>()
            .Configure(o =>
            {
                o.Authority = "https://fake";
                o.ClientId = "fake";
                o.ClientSecret = "fake";
                o.CallbackUrl = "https://fake/callback";
                o.PostLoginRedirectUrl = "https://fake/redirect";
            });

        return services.BuildServiceProvider();
    }

    // ------------------------------------------------------------
    // TESTS
    // ------------------------------------------------------------

    [Fact]
    public void DiscoverSteps_finds_all_DI_registered_steps()
    {
        var sp = BuildServiceProvider();

        var builder = new AuthCallbackPipelineBuilder(sp);
        var pipeline = builder.Build(); // Discovery is enabled by default

        pipeline.Steps.Count.Should().Be(9);
    }

    [Fact]
    public void DiscoverSteps_sorts_steps_by_category_order()
    {
        // Arrange
        var sp = BuildServiceProvider();

        // Act
        var pipeline = new AuthCallbackPipelineBuilder(sp)
            .Build(discoveryEnabled: true, validationEnabled: true);


        // Assert
        var actualCategoryOrder = pipeline.Steps
            .Select((s, i) => s.Metadata.Category)
            .Distinct()
            .Select((c, i) => new { Category = c, Index = i });

        var outOfOrder = AuthCallbackPipelineBuilder.DefaultCategoryOrder
            .Join(actualCategoryOrder, expected => expected, actual => actual.Category, (expected, actual) => actual.Index)
            .Where((actualIndex, expectedIndex) => actualIndex != expectedIndex)
            .ToList();

        outOfOrder.Should().BeEmpty(
            "the pipeline must follow the builder's defined category order");
    }

    [Fact]
    public void DiscoverSteps_throws_if_required_category_missing()
    {
        var services = new ServiceCollection();

        // Register only some steps
        services.AddScoped<IAuthCallbackStep, ExchangeCodeStep>();
        services.AddScoped<IAuthCallbackStep, FetchUserStep>();

        // Register required dependencies for those steps
        services.AddSingleton<IAuthClient, FakeAuthClient>();

        var sp = services.BuildServiceProvider();

        var builder = new AuthCallbackPipelineBuilder(sp);

        Action act = () => builder.Build();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*must appear exactly once*");
    }
}

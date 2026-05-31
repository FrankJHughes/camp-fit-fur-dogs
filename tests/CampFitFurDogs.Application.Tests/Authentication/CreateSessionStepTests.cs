using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class CreateSessionStepTests
{
    // ------------------------------------------------------------
    // Fake repository (no mocking framework)
    // ------------------------------------------------------------
    private sealed class FakeSessionRepository : ISessionRepository
    {
        public Session? Created { get; private set; }

        public Task CreateAsync(Session session)
        {
            Created = session;
            return Task.CompletedTask;
        }

        public Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash)
        {
            return Task.FromResult<Session?>(null);
        }

        public Task RevokeAsync(SessionTokenHash tokenHash)
        {
            return Task.CompletedTask;
        }
    }

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------
    private const string ValidHash =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    private static AuthCallbackContext MakeContext(
        Guid? customerId = null,
        SessionTokenHash? tokenHash = null,
        DateTimeOffset? now = null)
    {
        return new AuthCallbackContext("dummy-code")
        {
            CustomerId = customerId,
            TokenHash = tokenHash,
            Now = now ?? new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero)
        };
    }

    private static FakeSessionRepository FakeRepo() => new FakeSessionRepository();

    // ------------------------------------------------------------
    // 1. Creates a session and stores it in ctx.Session
    // ------------------------------------------------------------
    [Fact]
    public async Task Creates_session_and_sets_ctx_Session()
    {
        var customerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var tokenHash = SessionTokenHash.From(ValidHash);

        var ctx = MakeContext(customerId, tokenHash);
        var repo = FakeRepo();

        var step = new CreateSessionStep(repo);

        await step.ExecuteAsync(ctx, CancellationToken.None);

        ctx.Session.Should().NotBeNull();
        ctx.Session!.OwnerId.Should().Be(CustomerId.From(customerId));
        ctx.Session.TokenHash.Should().Be(tokenHash);
        ctx.Session.CreatedAt.Should().Be(ctx.Now);
    }

    // ------------------------------------------------------------
    // 2. Calls repository.CreateAsync with the created session
    // ------------------------------------------------------------
    [Fact]
    public async Task Calls_repository_with_created_session()
    {
        var customerId = Guid.NewGuid();
        var tokenHash = SessionTokenHash.From(ValidHash);

        var ctx = MakeContext(customerId, tokenHash);
        var repo = FakeRepo();

        var step = new CreateSessionStep(repo);

        await step.ExecuteAsync(ctx, CancellationToken.None);

        repo.Created.Should().NotBeNull();
        repo.Created.Should().Be(ctx.Session);
    }

    // ------------------------------------------------------------
    // 3. Throws when CustomerId is missing
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_customer_id_throws()
    {
        var ctx = MakeContext(customerId: null, tokenHash: SessionTokenHash.From(ValidHash));
        var repo = FakeRepo();

        var step = new CreateSessionStep(repo);

        var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // ------------------------------------------------------------
    // 4. Throws when TokenHash is missing
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_token_hash_throws()
    {
        var ctx = MakeContext(customerId: Guid.NewGuid(), tokenHash: null);
        var repo = FakeRepo();

        var step = new CreateSessionStep(repo);

        var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // ------------------------------------------------------------
    // 5. Uses ctx.Now as the session timestamp
    // ------------------------------------------------------------
    [Fact]
    public async Task Uses_ctx_Now_for_createdAt()
    {
        var now = new DateTimeOffset(2030, 5, 1, 8, 30, 0, TimeSpan.Zero);

        var ctx = MakeContext(
            customerId: Guid.NewGuid(),
            tokenHash: SessionTokenHash.From(ValidHash),
            now: now
        );

        var repo = FakeRepo();
        var step = new CreateSessionStep(repo);

        await step.ExecuteAsync(ctx, CancellationToken.None);

        ctx.Session!.CreatedAt.Should().Be(now);
    }
}

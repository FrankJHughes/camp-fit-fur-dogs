using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Pipeline.Steps
;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class CreateSessionStepTests
{
    private const string ValidHash =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    private static AuthCallbackContext MakeContext(
        Guid? customerId = null,
        SessionTokenHash? tokenHash = null,
        DateTimeOffset? now = null)
    {
        return new AuthCallbackContext(
            Code: "dummy-code",
            CustomerId: customerId,
            TokenHash: tokenHash,
            Now: now ?? new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero)
        );
    }

    private static FakeSessionRepository FakeRepo() => new FakeSessionRepository();

    // ------------------------------------------------------------
    // 1. Creates a session and returns updated context
    // ------------------------------------------------------------
    [Fact]
    public async Task Creates_session_and_sets_ctx_Session()
    {
        var customerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var tokenHash = SessionTokenHash.From(ValidHash);

        var ctx = MakeContext(customerId, tokenHash);
        var repo = FakeRepo();
        var uow = new FakeUnitOfWork();

        var step = new CreateSessionStep(repo, uow);

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        updated.Session.Should().NotBeNull();
        updated.Session!.OwnerId.Should().Be(CustomerId.From(customerId));
        updated.Session.TokenHash.Should().Be(tokenHash);
        updated.Session.CreatedAt.Should().Be(ctx.Now);

        uow.CommitCount.Should().Be(1);
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
        var uow = new FakeUnitOfWork();

        var step = new CreateSessionStep(repo, uow);

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        repo.CreatedSessions.Should().ContainSingle();
        repo.CreatedSessions.Single().Should().Be(updated.Session);

        uow.CommitCount.Should().Be(1);
    }

    // ------------------------------------------------------------
    // 3. Throws when CustomerId is missing
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_customer_id_throws()
    {
        var ctx = MakeContext(customerId: null, tokenHash: SessionTokenHash.From(ValidHash));
        var repo = FakeRepo();
        var uow = new FakeUnitOfWork();

        var step = new CreateSessionStep(repo, uow);

        var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.CommitCount.Should().Be(0);
    }

    // ------------------------------------------------------------
    // 4. Throws when TokenHash is missing
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_token_hash_throws()
    {
        var ctx = MakeContext(customerId: Guid.NewGuid(), tokenHash: null);
        var repo = FakeRepo();
        var uow = new FakeUnitOfWork();

        var step = new CreateSessionStep(repo, uow);

        var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.CommitCount.Should().Be(0);
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
        var uow = new FakeUnitOfWork();

        var step = new CreateSessionStep(repo, uow);

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        updated.Session!.CreatedAt.Should().Be(now);
        uow.CommitCount.Should().Be(1);
    }
}

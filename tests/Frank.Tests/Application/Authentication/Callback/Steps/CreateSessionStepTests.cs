using Frank.Abstractions.UnitOfWork;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Application.Identity.Callback.Steps;
using Frank.Domain.Sessions;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace Frank.Application.Tests.Authentication.Callback.Steps;

public sealed class CreateSessionStepTests
{
    private const string ValidHash =
        "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    private sealed class FakeSessionRepository : ISessionRepository
    {
        public Session? Created { get; private set; }
        public SessionTokenHash? RevokedHash { get; private set; }
        public SessionTokenHash? LookupHash { get; private set; }
        public Session? LookupResult { get; set; }

        public Task CreateAsync(Session session, CancellationToken ct)
        {
            Created = session;
            return Task.CompletedTask;
        }

        public Task<Session?> GetByTokenHashAsync(SessionTokenHash hash, CancellationToken ct)
        {
            LookupHash = hash;
            return Task.FromResult(LookupResult);
        }

        public Task RevokeAsync(SessionTokenHash hash, CancellationToken ct)
        {
            RevokedHash = hash;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeFrankIdentityUnitOfWork : IFrankIdentityUnitOfWork
    {
        public bool Committed { get; private set; }

        public Task<int> CommitAsync(CancellationToken ct)
        {
            Committed = true;
            return Task.FromResult(1);
        }
    }

    [Fact]
    public async Task ExecuteAsync_CreatesSession_AndSetsSessionId()
    {
        var repo = new FakeSessionRepository();
        var uow = new FakeFrankIdentityUnitOfWork();
        var step = new CreateSessionStep(repo, uow);

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            TokenHash = ValidHash
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        repo.Created.Should().NotBeNull();
        uow.Committed.Should().BeTrue();
        result.SessionId.Should().NotBeNull();
    }

    [Fact]
    public void CanExecute_OnlyWhenUserIdAndTokenHashAreSet_AndSessionIdIsNull()
    {
        var step = new CreateSessionStep(new FakeSessionRepository(), new FakeFrankIdentityUnitOfWork());

        var valid = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.NewGuid(),
            TokenHash = ValidHash,
            SessionId = null
        };

        step.CanExecute(valid).Should().BeTrue();
        step.CanExecute(valid with { UserId = null }).Should().BeFalse();
        step.CanExecute(valid with { TokenHash = null }).Should().BeFalse();
        step.CanExecute(valid with { SessionId = Guid.NewGuid() }).Should().BeFalse();
    }
}

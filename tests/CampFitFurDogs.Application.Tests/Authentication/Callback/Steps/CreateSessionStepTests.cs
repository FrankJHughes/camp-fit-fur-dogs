using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Authentication.Callback.Steps;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;
using CampFitFurDogs.Domain.Authentication.Sessions;
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

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

        public Task CreateAsync(Session session)
        {
            Created = session;
            return Task.CompletedTask;
        }

        public Task<Session?> GetByTokenHashAsync(SessionTokenHash hash)
        {
            LookupHash = hash;
            return Task.FromResult(LookupResult);
        }

        public Task RevokeAsync(SessionTokenHash hash)
        {
            RevokedHash = hash;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
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
        var uow = new FakeUnitOfWork();
        var step = new CreateSessionStep(repo, uow);

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            CustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            TokenHash = ValidHash
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        repo.Created.Should().NotBeNull();
        uow.Committed.Should().BeTrue();
        result.SessionId.Should().NotBeNull();
    }

    [Fact]
    public void CanExecute_OnlyWhenCustomerIdAndTokenHashAreSet_AndSessionIdIsNull()
    {
        var step = new CreateSessionStep(new FakeSessionRepository(), new FakeUnitOfWork());

        var valid = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            CustomerId = Guid.NewGuid(),
            TokenHash = ValidHash,
            SessionId = null
        };

        step.CanExecute(valid).Should().BeTrue();
        step.CanExecute(valid with { CustomerId = null }).Should().BeFalse();
        step.CanExecute(valid with { TokenHash = null }).Should().BeFalse();
        step.CanExecute(valid with { SessionId = Guid.NewGuid() }).Should().BeFalse();
    }
}

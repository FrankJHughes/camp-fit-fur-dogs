using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Application.Callback.Save.Steps;
using Frank.Identity.Domain.Sessions;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace Frank.Identity.Application.Tests.Callback.Save.Steps;

public sealed class CreateSessionStepTests
{
    private const string ValidHash =
        "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    // NEW: Slice-specific writer instead of ISessionRepository
    private sealed class FakeCreateSessionWriter : ICreateSessionWriter
    {
        public Session? Created { get; private set; }

        public Task WriteAsync(Session session, CancellationToken ct)
        {
            Created = session;
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
        var writer = new FakeCreateSessionWriter();
        var uow = new FakeFrankIdentityUnitOfWork();
        var step = new CreateSessionStep(writer, uow);

        var ctx = new CallbackSaveContext
        {
            External = FakeOidcCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            TokenHash = ValidHash
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        writer.Created.Should().NotBeNull();
        uow.Committed.Should().BeTrue();
        result.SessionId.Should().NotBeNull();
    }

    [Fact]
    public void CanExecute_OnlyWhenUserIdAndTokenHashAreSet_AndSessionIdIsNull()
    {
        var step = new CreateSessionStep(new FakeCreateSessionWriter(), new FakeFrankIdentityUnitOfWork());

        var valid = new CallbackSaveContext
        {
            External = FakeOidcCallbackResult.Create(),
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

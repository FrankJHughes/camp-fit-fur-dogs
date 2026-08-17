using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Users.CreateUser;
using Frank.Identity.Domain.Users;
using Frank.TestUtilities.Fakes;
using Frank.TestUtilities.Fixtures;

namespace Frank.Tests.Application.Users.CreateUser;

public class CreateUserHandlerTests
{
    private sealed class FakeCreateUserWriter : ICreateUserWriter
    {
        public List<User> Users { get; } = new();

        public Task WriteAsync(User user, CancellationToken ct)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }
    }

    private readonly FakeCreateUserWriter _writer = new();
    private readonly FakeFrankIdentityUnitOfWork _unitOfWork = new();
    private readonly CreateUserCommandHandler _handler;

    public CreateUserHandlerTests()
    {
        _handler = new CreateUserCommandHandler(_writer, _unitOfWork);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑1: Valid EXTERNAL user persists and returns ID
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidExternalUser_PersistsUserAndReturnsId()
    {
        var command = new CreateUserCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            ExternalId: "auth0|abc123"
        );

        var userId = await _handler.HandleAsync(command, CancellationToken.None);

        userId.Should().NotBe(Guid.Empty);
        _writer.Users.Should().HaveCount(1);

        var user = _writer.Users[0];
        user.ExternalId.Value.Should().Be("auth0|abc123");
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑2: Valid command commits unit of work
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var command = new CreateUserCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            ExternalId: "auth0|xyz789"
        );

        await _handler.HandleAsync(command, CancellationToken.None);

        _unitOfWork.Committed.Should().BeTrue();
        _unitOfWork.CommitCount.Should().Be(1);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑4: Cancellation token is honored
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var command = new CreateUserCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            ExternalId: "auth0|cancel"
        );

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(command, cts.Token));
    }
}

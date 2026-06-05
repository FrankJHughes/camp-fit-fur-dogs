using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Customers.CreateCustomer;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Customers.Exceptions;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Application.Tests.Customers.CreateCustomer;

public class CreateCustomerHandlerTests
{
    private readonly FakeCustomerRepository _repo = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTests()
    {
        _handler = new CreateCustomerHandler(_repo, _unitOfWork);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑1: Valid LOCAL customer persists and returns ID
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidLocalCustomer_PersistsCustomerAndReturnsId()
    {
        // Hash the password exactly like the API layer does
        var hashedPassword = PasswordHash.Create(PasswordFixtures.Plain).Value;

        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            Password: hashedPassword,
            ExternalAuthProviderId: null);

        var customerId = await _handler.HandleAsync(command, CancellationToken.None);

        customerId.Should().NotBe(Guid.Empty);
        _repo.Customers.Should().HaveCount(1);

        var customer = _repo.Customers[0];
        customer.ExternalAuthProviderId.Should().BeNull();
        customer.PasswordHash.Should().NotBeNull();
        customer.PasswordHash!.Value.Should().StartWith("$2"); // BCrypt prefix
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑2: Valid EXTERNAL customer persists and returns ID
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidExternalCustomer_PersistsCustomerAndReturnsId()
    {
        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            Password: null, // external identity → no password
            ExternalAuthProviderId: "auth0|abc123");

        var customerId = await _handler.HandleAsync(command, CancellationToken.None);

        customerId.Should().NotBe(Guid.Empty);
        _repo.Customers.Should().HaveCount(1);

        var customer = _repo.Customers[0];
        customer.ExternalAuthProviderId!.Value.Should().Be("auth0|abc123");
        customer.PasswordHash.Should().BeNull();
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑3: Valid command commits unit of work
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var hashedPassword = PasswordHash.Create(PasswordFixtures.Plain).Value;

        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            Password: hashedPassword,
            ExternalAuthProviderId: null);

        await _handler.HandleAsync(command, CancellationToken.None);

        _unitOfWork.Committed.Should().BeTrue();
        _unitOfWork.CommitCount.Should().Be(1);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑4: Duplicate email throws and does not commit twice
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsAndDoesNotCommitTwice()
    {
        var email = EmailFixtures.Unique().Value;
        var hashedPassword = PasswordHash.Create(PasswordFixtures.Plain).Value;

        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: email,
            Phone: PhoneNumberFixtures.Valid,
            Password: hashedPassword,
            ExternalAuthProviderId: null);

        // First call succeeds
        await _handler.HandleAsync(command, CancellationToken.None);

        // Second call should throw
        await Assert.ThrowsAsync<EmailAlreadyExistsException>(() =>
            _handler.HandleAsync(command, CancellationToken.None));

        _unitOfWork.CommitCount.Should().Be(1);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑5: Cancellation token is honored
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var hashedPassword = PasswordHash.Create(PasswordFixtures.Plain).Value;

        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            Password: hashedPassword,
            ExternalAuthProviderId: null);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(command, cts.Token));
    }
}

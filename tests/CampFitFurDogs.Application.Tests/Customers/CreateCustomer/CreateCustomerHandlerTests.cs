using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Customers.CreateCustomer;
using CampFitFurDogs.Application.Tests.Fakes;
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
    // AC‑1: Valid EXTERNAL customer persists and returns ID
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidExternalCustomer_PersistsCustomerAndReturnsId()
    {
        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            ExternalId: "auth0|abc123"
        );

        var customerId = await _handler.HandleAsync(command, CancellationToken.None);

        customerId.Should().NotBe(Guid.Empty);
        _repo.Customers.Should().HaveCount(1);

        var customer = _repo.Customers[0];
        customer.ExternalId.Value.Should().Be("auth0|abc123");
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑2: Valid command commits unit of work
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var command = new CreateCustomerCommand(
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

        var command = new CreateCustomerCommand(
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

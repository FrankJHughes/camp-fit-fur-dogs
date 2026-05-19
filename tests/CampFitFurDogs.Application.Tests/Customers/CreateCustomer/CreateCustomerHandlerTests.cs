using FluentAssertions;

using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Customers.CreateCustomer;
using CampFitFurDogs.Application.Tests.Fakes;

using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;

using CampFitFurDogs.Domain.Customers;

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
    // AC‑1: Valid command persists customer and returns ID
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_PersistsCustomerAndReturnsId()
    {
        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: EmailFixtures.Unique().Value,
            Phone: PhoneNumberFixtures.Valid,
            Password: PasswordFixtures.Plain);

        var customerId = await _handler.HandleAsync(command, CancellationToken.None);

        customerId.Should().NotBe(Guid.Empty);
        _repo.Customers.Should().HaveCount(1);
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
            Password: PasswordFixtures.Plain);

        await _handler.HandleAsync(command, CancellationToken.None);

        _unitOfWork.Committed.Should().BeTrue();
        _unitOfWork.CommitCount.Should().Be(1);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑3: Duplicate email throws and does not commit twice
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsAndDoesNotCommit()
    {
        var email = EmailFixtures.Unique().Value;

        var command = new CreateCustomerCommand(
            FirstName: NameFixtures.DefaultFirst,
            LastName: NameFixtures.DefaultLast,
            Email: email,
            Phone: PhoneNumberFixtures.Valid,
            Password: PasswordFixtures.Plain);

        // First call succeeds
        await _handler.HandleAsync(command, CancellationToken.None);

        // Second call should throw
        await Assert.ThrowsAsync<EmailAlreadyExistsException>(
            () => _handler.HandleAsync(command, CancellationToken.None));

        _unitOfWork.CommitCount.Should().Be(1);
    }
}

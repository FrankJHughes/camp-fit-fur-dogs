// tests/CampFitFurDogs.Application.Tests/Customers/CreateCustomer/CreateCustomerHandlerTests.cs
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Customers.CreateCustomer;
using CampFitFurDogs.Application.Tests.Fakes;
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

    [Fact]
    public async Task Handle_ValidCommand_PersistsCustomerAndReturnsId()
    {
        var command = new CreateCustomerCommand(
            FirstName: "Jane",
            LastName: "Doe",
            Email: "jane@example.com",
            Phone: "555-123-4567",
            Password: "ValidP@ss1");

        var customerId = await _handler.HandleAsync(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, customerId);
        Assert.Single(_repo.Customers);
    }

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var command = new CreateCustomerCommand(
            FirstName: "Jane",
            LastName: "Doe",
            Email: "jane@example.com",
            Phone: "555-123-4567",
            Password: "ValidP@ss1");

        await _handler.HandleAsync(command, CancellationToken.None);

        Assert.True(_unitOfWork.Committed);
        Assert.Equal(1, _unitOfWork.CommitCount);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsAndDoesNotCommit()
    {
        var command = new CreateCustomerCommand(
            FirstName: "Jane",
            LastName: "Doe",
            Email: "jane@example.com",
            Phone: "555-123-4567",
            Password: "ValidP@ss1");

        await _handler.HandleAsync(command, CancellationToken.None);

        await Assert.ThrowsAsync<EmailAlreadyExistsException>(
            () => _handler.HandleAsync(command, CancellationToken.None));

        Assert.Equal(1, _unitOfWork.CommitCount);
    }

}

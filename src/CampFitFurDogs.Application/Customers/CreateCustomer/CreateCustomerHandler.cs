using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application.Customers.CreateCustomer;

public sealed class CreateCustomerHandler
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        // Fake implementation for TDD step
        return Task.FromResult(Guid.NewGuid());
    }
}

using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Domain.Customers.Exceptions;
using FluentValidation;
using SharedKernel.Abstractions;

public sealed class FakeCreateCustomerHandler
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public CreateCustomerCommand? LastCommand { get; private set; }
    public Guid ResultToReturn { get; set; } = Guid.NewGuid();
    public Exception? ExceptionToThrow { get; set; }

    private readonly IValidator<CreateCustomerCommand> _validator;

    public FakeCreateCustomerHandler()
    {
        _validator = new CreateCustomerCommandValidator();
    }

    public Task<Guid> HandleAsync(CreateCustomerCommand command, CancellationToken ct)
    {
        LastCommand = command;

        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        // Run the real validator
        var result = _validator.Validate(command);

        if (!result.IsValid)
            throw new MissingIdentitySourceException(result.Errors.First().ErrorMessage);

        return Task.FromResult(ResultToReturn);
    }
}

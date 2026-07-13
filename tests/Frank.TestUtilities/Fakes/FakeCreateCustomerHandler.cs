using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Frank.Core.Application.Abstractions.Command;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Users.CreateUser;
using Frank.Identity.Domain.Users.Exceptions;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeCreateUserHandler
    : ICommandHandler<CreateUserCommand, Guid>
{
    public CreateUserCommand? LastCommand { get; private set; }
    public Guid ResultToReturn { get; set; } = Guid.NewGuid();
    public Exception? ExceptionToThrow { get; set; }

    private readonly IValidator<CreateUserCommand> _validator;

    public FakeCreateUserHandler()
    {
        _validator = new CreateUserCommandValidator();
    }

    public Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken ct)
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

using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Application;

public sealed class ValidatingCommandHandler
    : ICommandHandler<FakeStringResponseCommand, string>
{
    private readonly IValidator<FakeStringResponseCommand> _validator;
    private readonly ICommandHandler<FakeStringResponseCommand, string> _inner;

    public ValidatingCommandHandler(
        IValidator<FakeStringResponseCommand> validator,
        ICommandHandler<FakeStringResponseCommand, string> inner)
    {
        _validator = validator;
        _inner = inner;
    }

    public async Task<string> Handle(FakeStringResponseCommand command, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(command, ct);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return await _inner.Handle(command, ct);
    }
}

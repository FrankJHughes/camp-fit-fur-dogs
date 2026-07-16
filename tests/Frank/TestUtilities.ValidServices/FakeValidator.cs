namespace Frank.TestUtilities.ValidServices;

public sealed class FakeValidator : AbstractValidator<FakeCommand>
{
    public FakeValidator()
    {
        RuleFor(_ => _).NotNull();
    }
}

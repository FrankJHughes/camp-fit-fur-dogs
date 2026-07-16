namespace Frank.TestUtilities.ValidServices;

public sealed class AnotherFakeValidator : AbstractValidator<FakeCommand>
{
    public AnotherFakeValidator()
    {
        RuleFor(_ => _).NotNull();
    }
}

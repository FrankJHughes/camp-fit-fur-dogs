using FluentValidation;

namespace SharedKernel.Tests.Fakes;

public sealed class FakeValidator1 : AbstractValidator<FakeCommand>
{
    public FakeValidator1()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public sealed class FakeValidator2 : AbstractValidator<FakeCommand>
{
    public FakeValidator2()
    {
        RuleFor(x => x.Name).MinimumLength(3);
    }
}

public abstract class AbstractFakeValidator : AbstractValidator<FakeCommand>
{
}

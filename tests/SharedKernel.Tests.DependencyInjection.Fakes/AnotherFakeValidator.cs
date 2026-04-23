using FluentValidation;

namespace SharedKernel.Tests.DependencyInjection.Fakes;

public sealed class AnotherFakeValidator : AbstractValidator<FakeCommand>
{
    public AnotherFakeValidator()
    {
        RuleFor(_ => _).NotNull();
    }
}

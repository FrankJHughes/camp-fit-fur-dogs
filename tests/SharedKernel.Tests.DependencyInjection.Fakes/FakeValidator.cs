using FluentValidation;

namespace SharedKernel.Tests.DependencyInjection.Fakes;

public sealed class FakeValidator : AbstractValidator<FakeCommand>
{
    public FakeValidator()
    {
        RuleFor(_ => _).NotNull();
    }
}

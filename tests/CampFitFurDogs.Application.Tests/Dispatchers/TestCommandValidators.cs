using FluentValidation;
using TestDoubles.Dispatchers;

namespace CampFitFurDogs.Application.Tests.Dispatchers;

public sealed class AlwaysValidCommandValidator : AbstractValidator<TestCommand>
{
    public AlwaysValidCommandValidator()
    {
        RuleFor(x => x.Value).NotNull();
    }
}

public sealed class AlwaysInvalidCommandValidator : AbstractValidator<TestCommand>
{
    public AlwaysInvalidCommandValidator()
    {
        RuleFor(x => x.Value)
            .Must(_ => false)
            .WithMessage("Invalid");
    }
}

using FluentValidation;
using TestDoubles.Dispatchers;

namespace CampFitFurDogs.Application.Tests.Dispatchers;

public sealed class AlwaysInvalidQueryValidator : AbstractValidator<TestQuery>
{
    public AlwaysInvalidQueryValidator()
    {
        RuleFor(x => x.Value)
            .Must(_ => false)
            .WithMessage("Invalid");
    }
}

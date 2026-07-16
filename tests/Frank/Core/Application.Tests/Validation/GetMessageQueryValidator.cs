
using Frank.Core.Application.Tests.Slices;

namespace Frank.Core.Application.Tests.Validation;

public sealed class GetMessageQueryValidator : AbstractValidator<GetMessageQuery>
{
    public GetMessageQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be positive.");
    }
}


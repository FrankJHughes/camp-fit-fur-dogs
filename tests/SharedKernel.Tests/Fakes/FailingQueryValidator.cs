using FluentValidation;

namespace SharedKernel.Tests.Fakes;

public sealed class FailingQueryValidator<T> : AbstractValidator<T>
{
    public FailingQueryValidator()
    {
        RuleFor(x => x)
            .Custom((_, context) =>
            {
                context.AddFailure("Validation failed");
            });
    }
}

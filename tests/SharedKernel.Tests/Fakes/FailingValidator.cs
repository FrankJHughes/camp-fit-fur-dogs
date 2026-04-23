using FluentValidation;

namespace SharedKernel.Tests.Fakes;

public sealed class FailingValidator<T> : AbstractValidator<T>
{
    public FailingValidator()
    {
        RuleFor(x => x)
            .Custom((_, context) =>
            {
                context.AddFailure("Validation failed");
            });
    }
}

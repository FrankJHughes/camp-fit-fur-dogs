
using Frank.Core.Application.Tests.Slices;

namespace Frank.Core.Application.Tests.Validation;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Message text is required.");
    }
}


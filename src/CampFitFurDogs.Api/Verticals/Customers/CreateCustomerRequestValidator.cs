using FluentValidation;

namespace CampFitFurDogs.Api.Verticals.Customers;

public sealed class CreateCustomerRequestValidator
    : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        // First name
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("A first name is required.")
            .MaximumLength(100);

        // Last name
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("A last name is required.")
            .MaximumLength(100);

        // Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("An email address is required.")
            .EmailAddress()
            .WithMessage("Please provide a valid email address.");

        // Phone (optional but must be valid if provided)
        RuleFor(x => x.Phone)
            .Must(BeValidPhone)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number must contain between 10 and 15 digits.");

        // Password (plaintext, only required for local identity)
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Password must be at least 8 characters long.");
    }

    private static bool BeValidPhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return digits.Length >= 10 && digits.Length <= 15;
    }
}

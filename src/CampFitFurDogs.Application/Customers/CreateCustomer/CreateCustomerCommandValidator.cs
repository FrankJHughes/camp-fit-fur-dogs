using FluentValidation;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;

public sealed class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        // First Name
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-Za-z' -]+$")
            .WithMessage("First name contains invalid characters.");

        // Last Name
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-Za-z' -]+$")
            .WithMessage("Last name contains invalid characters.");

        // Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Matches(@"^(?!\.)[A-Za-z0-9._%+-]+@(?!-)([A-Za-z0-9-]+\.)+[A-Za-z]{2,63}$")
            .WithMessage("Email format is invalid.");

        // Phone
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^[0-9+\-\s().]+$")
            .WithMessage("Phone number contains invalid characters.");

        // Password
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Za-z]").WithMessage("Password must contain at least one letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.");
    }
}

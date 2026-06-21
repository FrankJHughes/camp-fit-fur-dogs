using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using FluentValidation;

public sealed class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        // ─────────────────────────────────────────────────────────────
        // Identity Source Rules (Semantic Validation)
        // ─────────────────────────────────────────────────────────────

        // External identity is required
        RuleFor(x => x.ExternalId)
            .NotEmpty()
            .WithMessage("External provider ID is required.");

        // Must be in the format "provider|id"
        RuleFor(x => x.ExternalId)
            .Must(id => id.Contains('|'))
            .WithMessage("External provider ID must be in the format 'provider|id'.");

        // ─────────────────────────────────────────────────────────────
        // NOTE:
        // We do NOT validate FirstName, LastName, Email, Phone here.
        // Those are syntactic rules handled by the REQUEST VALIDATOR.
        // The DOMAIN enforces invariants via Value Objects.
        // ─────────────────────────────────────────────────────────────
    }
}
